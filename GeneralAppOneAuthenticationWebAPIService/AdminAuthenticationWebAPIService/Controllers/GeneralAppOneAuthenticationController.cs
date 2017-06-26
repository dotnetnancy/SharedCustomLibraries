using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using DotNetNancy.GeneralApps.WebApi.Common;
using DotNetNancy.GeneralApps.GeneralAppOne.Business;
using DotNetNancy.GeneralApps.GeneralAppOne.Models;

namespace GeneralAppOneAuthenticationWebAPIService.Controllers
{
    public class GeneralAppOneAuthenticationController : BaseController
    {
        IAuthenticationBusiness _authorizedUserBusiness =
            new AuthenticationBusiness();

        [HttpPost]
        [Route("api/GeneralAppOneAuthentication/GeneralAppOneAuthenticate")]
        [ResponseType(typeof(LoginResult))]
        public JsonNetResult GeneralAppOneAuthenticate([FromBody] LoginDetails credentials)
        {
            var auth = new LoginResult() { UserInfo = new UserLoginInfo() };
            auth.UserInfo.loginstatus = false;

            auth = TryLogin(credentials);

             List<JsonErrorModel> errorList = new List<JsonErrorModel>();
            if (!auth.UserInfo.loginstatus)
                errorList.Add(new DotNetNancy.GeneralApps.WebApi.Common.JsonErrorModel
                {
                    ErrorMessage = "Authentication Failed, please check credientials."
                });

          
            JsonNetResult jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.Data = (auth.UserInfo.loginstatus) ? auth : null;
             jsonNetResult.DomainSuccess = auth.UserInfo.loginstatus;
            jsonNetResult.DomainErrors = errorList.Any() ? errorList : null;
            return jsonNetResult;
        }
        private LoginResult TryLogin(LoginDetails loginDetails)
        {
            return _authorizedUserBusiness.AuthenticateUser(loginDetails);
        }

        [Authorize]
        [HttpGet]
        [Route("api/GeneralAppOneAuthentication/ProtectedGetMethod")]
        public JsonNetResult ProtectedGetMethod()
        {
            if (RequestContext != null && this.RequestContext.Principal != null)
            {
                var claimsPrincipal = (ClaimsPrincipal)RequestContext.Principal;
            }
            var jsonNetResult = new JsonNetResult() { Data = "ProtectedGetMethod was called" };
            return jsonNetResult;
        }


    }

}