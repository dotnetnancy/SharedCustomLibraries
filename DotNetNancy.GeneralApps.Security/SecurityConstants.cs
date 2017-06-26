using System.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace DotNetNancy.GeneralApps.Security
{
    public class SecurityConstants
    {
        public static byte[] KeyForHmacSha256 = null;

        //i hardcode this secret here but we could put this in a database or something like that, just did not want it in 
        //the config file in clear text.  We could put in config file encrypted in some way also i suppose
        public static readonly string secret = "DotNetNancy secret  auth string";


        static SecurityConstants()
        {
            var encodedSecret = Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Encode(secret);
            KeyForHmacSha256 = System.Text.Encoding.UTF8.GetBytes(encodedSecret);
        }
    }
}