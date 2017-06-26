using AuthorizedUser = DotNetNancy.GeneralApps.GeneralAppOne.Models.AuthorizedUser;
using DotNetNancy.Generic.Data.Core;
using DotNetNancy.GeneralApps.GeneralAppOne.Models;

namespace DotNetNancy.GeneralApps.GeneralAppOne.DataLayer
{
    public interface IAuthorizedUserRepository :
        IGenericDataRepository<AuthorizedUser, AuthenticationContextExample>
    {
        AuthorizedUser GetUserBySproc(LoginDetails details);
        AuthorizedUser GetUserByLinq(LoginDetails details);
        AuthorizedUser GetUserByBase(LoginDetails details);
    }
}
