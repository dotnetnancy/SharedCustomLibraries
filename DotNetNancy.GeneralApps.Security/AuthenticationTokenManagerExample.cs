using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace DotNetNancy.GeneralApps.Security
{
    [SuppressMessage("ReSharper", "SimplifyConditionalTernaryExpression")]
    public class AuthenticationTokenManager
    {
        public static async Task<string> CreateJwtTokenAsync(string username, long franchisorId)
        {
            var claimsList = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, username)               
            };

            var tokenHandler = new JwtSecurityTokenHandler() { };
            var sSKey = new InMemorySymmetricSecurityKey(SecurityConstants.KeyForHmacSha256);

            var jwtToken = tokenHandler.CreateToken(makeSecurityTokenDescriptor(sSKey, claimsList));
            return tokenHandler.WriteToken(jwtToken);
        }

        public static List<Claim> GetClaimsByToken(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Parse JWT from the Base64UrlEncoded wire form (<Base64UrlEncoded header>.<Base64UrlEncoded body>.<signature>)
            var parsedJwt = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;
            return ((parsedJwt != null) && (parsedJwt.Claims != null)) ? parsedJwt.Claims.ToList() : null;
        }

        public static void InvalidateJwtToken(string jwtToken)
        {
            AuthenticationTokenManager.InvalidateJwtToken(jwtToken);
        }

        public static ClaimsPrincipal ValidateJwtToken(string jwtToken, out JwtSecurityToken parsedJwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler() { };

            // Parse JWT from the Base64UrlEncoded wire form (<Base64UrlEncoded header>.<Base64UrlEncoded body>.<signature>)
            parsedJwt = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            var audience = ConfigurationManager.AppSettings["JwtAllowedAudience"] ?? "http://localhost:8080";
            var issuer = ConfigurationManager.AppSettings["JwtValidIssuer"] ?? "DotNetNancysolutions";
            var configValidateAudience = ConfigurationManager.AppSettings["JwtValidateAudience"];
            var configValidateIssuer = ConfigurationManager.AppSettings["JwtValidateIssuer"];
            bool validateAudience = configValidateAudience == null ? false : Convert.ToBoolean(configValidateAudience);
            bool validateIssuer = configValidateIssuer == null ? true : Convert.ToBoolean(configValidateIssuer);


            TokenValidationParameters validationParams =
                new TokenValidationParameters()
                {
                    ValidAudience = audience,
                    ValidateAudience = validateAudience,
                    ValidIssuer = issuer,
                    ValidateIssuer = validateIssuer,
                    IssuerSigningToken = new BinarySecretSecurityToken(SecurityConstants.KeyForHmacSha256),
                };
            System.IdentityModel.Tokens.SecurityToken securityToken = null;
            return tokenHandler.ValidateToken(jwtToken, validationParams, out securityToken);
        }

        private static SecurityTokenDescriptor makeSecurityTokenDescriptor(InMemorySymmetricSecurityKey sSKey, List<Claim> claimList)
        {
            var now = DateTime.UtcNow;
            Claim[] claims = claimList.ToArray();

            string lifespanString = ConfigurationManager.AppSettings["JwtLifeSpanInMinutes"];

            lifespanString = lifespanString ?? "30";

            int lifespanInMinutes = Convert.ToInt32(lifespanString);

            var audience = ConfigurationManager.AppSettings["JwtAllowedAudience"] ?? "http://localhost:8080";
            var issuer = ConfigurationManager.AppSettings["JwtValidIssuer"] ?? "DotNetNancysolutions";

            return new System.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                AppliesToAddress = audience,
                TokenIssuerName = issuer,
                Lifetime = new Lifetime(now, now.AddMinutes(lifespanInMinutes)),
                SigningCredentials = new System.IdentityModel.Tokens.SigningCredentials(sSKey,
                    "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                    "http://www.w3.org/2001/04/xmlenc#sha256"),
            };
        }

        public static IPrincipal GetPrincipal(string tokenString)
        {
            JwtSecurityToken parsedJwt = null;
            IPrincipal principal = AuthenticationTokenManager.ValidateJwtToken(tokenString, out parsedJwt);
            return principal;
        }
    }
}