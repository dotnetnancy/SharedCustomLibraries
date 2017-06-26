using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using NextGear.Dash.Generic.Model;

namespace AuthenticationWebAPIService
{
    public static class ODataConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes(); //This has to be called before the following OData mapping, so also before WebApi mapping

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            EntitySetConfiguration<AuthorizedUser> authorizedUsers =  builder.EntitySet<AuthorizedUser>("AuthorizedUser");
            
            config.MapODataServiceRoute("ODataRoute", "odata", builder.GetEdmModel());
            config.AddODataQueryFilter();
        }
    }
}
