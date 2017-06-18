using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DotNetNancy.Model;
using AuthorizedUser = DotNetNancy.Model.AuthorizedUser;
using UserLoginInfo = DotNetNancy.Model.UserLoginInfo;
using DotNetNancy.Generic.Data.Core;
using DotNetNancy.GeneralApps.Generic.DataLayer;

namespace DotNetNancy.DataLayer
{
    public interface IAuthorizedUserRepository :
        IGenericDataRepository<AuthorizedUser, AuthenticationContextExample>
    {
        AuthorizedUser GetUserBySproc(LoginDetails details);
        AuthorizedUser GetUserByLinq(LoginDetails details);
        AuthorizedUser GetUserByBase(LoginDetails details);
    }
}
