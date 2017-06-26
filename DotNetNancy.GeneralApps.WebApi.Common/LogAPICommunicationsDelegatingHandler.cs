using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using log4net;

namespace DotNetNancy.GeneralApps.WebApi.Common
{
    public class LogAPICommunicationsDelegatingHandler : System.Net.Http.DelegatingHandler
    {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //declared at class level in all classes requiring logging.
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //only log if debug is the switch set in config for log4net
            if (Log.IsDebugEnabled)
            {
                HttpStatusCode statusCode = HttpStatusCode.Accepted;

                Task<HttpResponseMessage> response;
                try
                {
                    //log the request information
                    Log.Debug(request.ToString());

                    response = base.SendAsync(request, cancellationToken);
                    //log the response information
                    Log.Debug(response.Result.ToString());

                    return response;
                }

                catch (Exception ex)
                {
                    statusCode = HttpStatusCode.InternalServerError;

                    //TODO: log the exception
                    Log.Debug(ex);
                }

                response = Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode));

                //log the response information
                Log.Debug(response.Result.ToString());
                return response;
            }
            else
            {
                //pass thru, don't log anything
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}
