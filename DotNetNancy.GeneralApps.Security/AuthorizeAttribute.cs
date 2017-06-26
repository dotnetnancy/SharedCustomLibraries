using System.Web.Http;
using System.Web.Http.Controllers;

namespace NextGear.Dash.Security
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string SecurityArea { get; set; }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            //TODO: Actual role based authorization checks.
            return true;
        }
    }
}