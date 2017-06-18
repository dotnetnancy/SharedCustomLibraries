using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DotNetNancy;
using DotNetNancy.Model;
using DotNetNancy.GeneralApps;


namespace DotNetNancy.GeneralApps.Authentication.Business
{
    public interface IAuthenticationBusiness
    {
        LoginResult AuthenticateUser(LoginDetails loginDetails);
    }
}
