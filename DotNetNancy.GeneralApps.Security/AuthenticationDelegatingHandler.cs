using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

//using System.IdentityModel.Tokens.Jwt;

namespace DotNetNancy.GeneralApps.Security
{
    public class AuthenticationJwtTokenValidationHandler : System.Net.Http.DelegatingHandler
    {
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }


        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;
            string token;

            var authHeader = request.Headers.Authorization;
            if (authHeader == null)
            {
                return base.SendAsync(request, cancellationToken);
            }

            if (!TryRetrieveToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode));
            }

            try
            {
                JwtSecurityToken parsedJwt = null;
                IPrincipal principal = AuthenticationTokenManager.ValidateJwtToken(token, out parsedJwt);

                //this works for both self hosted using owin and IIS hosted
                request.GetRequestContext().Principal = principal;

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenExpiredException)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (SecurityTokenValidationException)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (Exception)
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode));
        }
       
    }
}
