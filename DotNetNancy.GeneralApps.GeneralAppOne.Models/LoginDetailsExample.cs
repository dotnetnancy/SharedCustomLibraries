using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNancy.GeneralApps.GeneralAppOne.Models
{
    [DataContract]
    public class LoginDetails
    {
        
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
       
    }
}
