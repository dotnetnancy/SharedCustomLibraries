using DotNetNancy.GeneralApps.GeneralAppOne.Models;

namespace DotNetNancy.GeneralApps.GeneralAppOne.Business
{
    public interface IAuthenticationBusiness
    {
        LoginResult AuthenticateUser(LoginDetails loginDetails);
    }
}
