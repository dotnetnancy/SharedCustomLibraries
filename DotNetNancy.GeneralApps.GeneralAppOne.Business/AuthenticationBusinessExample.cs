using DotNetNancy.GeneralApps.GeneralAppOne.DataLayer;
using DotNetNancy.GeneralApps.GeneralAppOne.Models;
using log4net;
using System;
using System.Reflection;

namespace DotNetNancy.GeneralApps.GeneralAppOne.Business
{
    public class AuthenticationBusiness : IAuthenticationBusiness
    {
        private IAuthorizedUserRepository _authorizedUserRepository;

        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AuthenticationBusiness()
        {
            _authorizedUserRepository = new AuthorizedUserRepository();
        }

        public AuthenticationBusiness(IAuthorizedUserRepository authorizedUserRepository)
        {
            _authorizedUserRepository = authorizedUserRepository;
        }

        public LoginResult AuthenticateUser(LoginDetails loginDetails)
        {
            try
            {
               
                var auth = new LoginResult() {UserInfo = new UserLoginInfo()};
                auth.UserInfo.loginstatus = true;
                //these are 3 examples of how to use the generic repository, the context dbsets, or a sproc
                var user = _authorizedUserRepository.GetUserByBase(loginDetails);
                //var user = _authorizedUserRepository.GetUserByLinq(loginDetails);
                // var user = _authorizedUserRepository.GetUserBySproc(loginDetails);//if you are stuck with some very legacy code on sql side of house

                if ((auth.UserInfo != null) && ((auth.UserInfo != null)) && (auth.UserInfo.loginstatus == true))
                {
                    var tokenString = "this is a token string";//call some method to provide a valid token
                    auth.UserInfo.TokenID = tokenString;
                }

                return auth;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

    }
}
