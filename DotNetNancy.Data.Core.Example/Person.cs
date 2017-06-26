using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNancy.Model
{
    [DataContract]
    public class Person : IEntity
    {
        [DataMember]
        [Key]
        public long PersonId { get; set; }      

        [DataMember]
        public string PersonName { get; set; }      

        [DataMember]
        public string PersonType { get; set; }

        [DataMember]
        public string PersonNumber { get; set; }

        [DataMember]
        public string Address1 { get; set; }

        [DataMember]
        public string Address2 { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Zip { get; set; }

        [DataMember]
        public string BusinessPhone { get; set; }

        [DataMember]
        public string MobilePhone { get; set; }

        [DataMember]
        public string HomePhone { get; set; }

        [DataMember]
        public string Fax { get; set; }

        [DataMember]
        public string Pager { get; set; }

        [DataMember]
        public string OtherPhone { get; set; }

        [DataMember]
        public string TollFreeNumber { get; set; }

        [DataMember]
        public string WebSite { get; set; }

        [DataMember]
        public string CorrespondenceEmail { get; set; }

        [DataMember]
        public string EnquiryEmail { get; set; }

        [DataMember]
        public string SupportEmail { get; set; }

    

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public string AddedBy { get; set; }

        [DataMember]
        public DateTime? AddedDate { get; set; }

        [DataMember]
        public string UpdatedBy { get; set; }

        [DataMember]
        public DateTime? UpdatedDate { get; set; }

        [NotMapped]
        public EntityState EntityState { get; set; }

    }
}
