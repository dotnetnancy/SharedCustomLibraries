using System;
using System.Collections.Generic;
using System.Linq;
using AuthorizedUser = DotNetNancy.GeneralApps.GeneralAppOne.Models.AuthorizedUser;
using DotNetNancy.Generic.Data.Core;
using DotNetNancy.GeneralApps.GeneralAppOne.Models;

namespace DotNetNancy.GeneralApps.GeneralAppOne.DataLayer
{
    public class AuthorizedUserRepository :
        GenericDataRepository<AuthorizedUser, AuthenticationContextExample>, IAuthorizedUserRepository
    {
        /// <summary>
        /// you get the idea on this one there are other base methods and overloads including pass an
        /// instantiated context etc.
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public AuthorizedUser GetUserByBase(LoginDetails details)
        {
            try
            {
                var encryptedPassword = GetEncryptedPassword(details.Password);
                return GetSingle(x => x.UserName == details.UserName && x.Password == encryptedPassword);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }

        public AuthorizedUser GetUserByLinq(LoginDetails details)
        {
            try
            {
                var encryptedPassword = GetEncryptedPassword(details.Password);
                using (var context = new AuthenticationContextExample())
                {
                    var userQuery = from users in context.AuthorizedUsers
                                    where users.UserName == details.UserName && users.Password == encryptedPassword
                                    select users;

                    return userQuery.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        private byte[] GetEncryptedPassword(string password)
        {
            return new byte[password.Length];
        }

        public AuthorizedUser GetUserBySproc(LoginDetails details)
        {
            try
            {
                Dictionary<string, object> parameterNamesToValues = new Dictionary<string, object>();
                parameterNamesToValues.Add("@UserName", details.UserName);
                parameterNamesToValues.Add("@Password", details.Password);
                var storedProcedureName = "GetUser";
                var result = ExecuteStoredProcedureFirstOrDefault<AuthorizedUser>
                    (storedProcedureName, parameterNamesToValues);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }
    }
}
