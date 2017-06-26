using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AuthenticationWebAPIService;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using DotNetNancy.GeneralApps.WebApi.Common;
using DotNetNancy.GeneralApps.GeneralAppOne.Models;

namespace GeneralAppOneAuthenticationTests
{
    [TestClass]
    public class GeneralAppOneAuthenticationWebApiTests
    {
            static string _baseAddress = "http://localhost:9000/";

        private static string userName = "GeneralAppOne"; 
        private static string password = "password";
     
        private IDisposable _webApp = null;

            [TestMethod]
            public void GeneralAppOneAuthenticate()
            {
                RunAsyncGeneralAppOneAuthenticate().Wait();
            }

            static async Task RunAsyncGeneralAppOneAuthenticate()
            {
                string controllerPath = "api/GeneralAppOneAuthentication/";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_baseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync(controllerPath);

                    // HTTP POST
                    var loginDetails = new LoginDetails()
                    {
                        UserName = userName,
                        Password = password
                    };
                    response = await client.PostAsJsonAsync(controllerPath + "GeneralAppOneAuthenticate/", loginDetails);
                    Assert.IsTrue(response.IsSuccessStatusCode);
                    var jsonNetResult = await response.Content.ReadAsAsync<JsonNetResult>();
                    var loginResult = JsonConvert.DeserializeObject<LoginResult>(jsonNetResult.Data.ToString());
                    Assert.IsNotNull(loginResult.UserInfo);
                    Assert.IsTrue(loginResult.UserInfo.loginstatus);

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                        loginResult.UserInfo.TokenID);
                    response = await client.GetAsync(controllerPath + "ProtectedGetMethod/");
                    Assert.IsTrue(response.IsSuccessStatusCode);
                    jsonNetResult = await response.Content.ReadAsAsync<JsonNetResult>();
                    var protectedMethodResult = jsonNetResult.Data.ToString();
                    Assert.IsFalse(String.IsNullOrEmpty(protectedMethodResult));
                }
            }

            [TestInitialize]
            public void Init()
            {
                _webApp = WebApp.Start<Startup>(url: _baseAddress);
            }

            [TestCleanup]
            public void TearDown()
            {
                _webApp.Dispose();
            }
        }

    }

