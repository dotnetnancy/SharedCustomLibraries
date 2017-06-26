using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DotNetNancy.Generic.Data.Core;

namespace DotNetNancy.GeneralApps.GeneralAppOne.Models
{
    [DataContract]
    public partial class AuthorizedUser : IEntity
    {
        [DataMember]
        public long AuthorizedUserId { get; set; }
     
        [DataMember]
        public byte[] Password { get; set; }

        [DataMember]
        public string UserName { get; set; }       
       
        [NotMapped] 
        public EntityState EntityState { get; set; }
    }
}
