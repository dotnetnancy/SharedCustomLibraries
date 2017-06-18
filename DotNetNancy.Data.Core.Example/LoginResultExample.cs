using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNancy.Model
{
    [DataContract]
    public sealed class LoginResult
    {
        [DataMember]
        public string ErrorOrWarningMessage { get; set; }

        // will be null on a failed login attempt
        [DataMember]
        public UserLoginInfo UserInfo { get; set; }
     
    }
}
