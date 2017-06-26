using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DotNetNancy.GeneralApps.GeneralAppOne.Models
{
    [DataContract]
    public partial class UserLoginInfo
    {
        [DataMember]
        public long UserId { get; set; }
        [DataMember]
        public string UserType { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public byte[] Password { get; set; }
        [DataMember]
        public string EmailWork { get; set; }
        [DataMember]
        public bool loginstatus { get; set; }
        [DataMember]
        public string TokenID { get; set; }
    }

}
