using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AuthenticationWebAPIService;
using Microsoft.Owin.Hosting;

namespace AuthenticationOwinSelfHosted
{
    class Program
    {
        public static string baseAddress = "http://localhost:9000/";
        public static IDisposable webapp = WebApp.Start<Startup>(url: baseAddress);
        static void Main()
        {
            // Start OWIN host notice the using block... it will go away 
            //at the end of the using
            //using (WebApp.Start<Startup>(url: baseAddress))
            //{

           
                // Create HttpCient and make a request to api/Authentication -
         
                //HttpClient client = new HttpClient();
                //This will get all the users in the system, 15000 or so - please narrow it down...
                //var response = client.GetAsync(baseAddress + "api/Authentication").Result;

                //Console.WriteLine(response);
                //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            //}
            Console.WriteLine("Your Services at base url:  " + baseAddress + " are running");
            Console.WriteLine("Ctrl + C OR Ctrl + break to exit");
            Console.ReadLine();
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            webapp.Dispose();
        } 
    }
}
