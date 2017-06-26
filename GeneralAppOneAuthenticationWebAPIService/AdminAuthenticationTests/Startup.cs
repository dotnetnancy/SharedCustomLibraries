using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.OData.Batch;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.Routing;
using log4net;
using log4net.Config;
using Microsoft.OData.Edm;
using Owin;
using AuthenticationWebAPIService;
using DotNetNancy.GeneralApps.Security;
using DotNetNancy.GeneralApps.WebApi.Common;
using DotNetNancy.GeneralApps.GeneralAppOne.Models;

namespace AuthenticationWebAPIService
{
    public class Startup
    {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 

        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            appBuilder.UseWebApi(config);
          
                WebApiConfig.Register(config);
                config.MessageHandlers.Add(new AuthenticationJwtTokenValidationHandler());
                config.MessageHandlers.Add(new LogAPICommunicationsDelegatingHandler());
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            SetUpLogging();

            Log.Debug("Authentication Web Api Service Application is starting");
        }

        private void SetUpLogging()
        {
            XmlConfigurator.Configure(); //only once
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.Namespace = "Demos";
            builder.ContainerName = "DefaultContainer";
            builder.EntitySet<AuthorizedUser>("AuthorizedUsers");
            var edmModel = builder.GetEdmModel();
            return edmModel;
        }
    }
}
