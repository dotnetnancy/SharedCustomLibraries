using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using log4net;
using log4net.Config;
using DotNetNancy.GeneralApps.WebApi.Common;
using DotNetNancy.GeneralApps.Security;

namespace AuthenticationWebAPIService
{
    public class Global : HttpApplication
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); 

        void Application_Start(object sender, EventArgs e)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(config =>
            {
                WebApiConfig.Register(config);
                config.MessageHandlers.Add(new AuthenticationJwtTokenValidationHandler());
                config.MessageHandlers.Add(new LogAPICommunicationsDelegatingHandler());
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            });
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            SetUpLogging();

            Log.Debug("GeneralAppOne Authentication Web Api Service Application is starting");
        }

        private void SetUpLogging()
        {
            XmlConfigurator.Configure(); //only once
        }

        void Application_End(object sender, EventArgs e)
        {
            Log.Debug("GeneralAppOne Authentication Web Api Service Application is ending");
        }
    }
}