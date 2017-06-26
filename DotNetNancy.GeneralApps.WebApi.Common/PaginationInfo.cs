using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNancy.GeneralApps.WebApi.Common
{
    public class PaginationInfo
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PreviousPage { get; set; }
        public int? NextPage { get; set; }
        public string PreviousPageLink { get; set; }
        public string NextPageLink { get; set; }
        public int MaxRows { get; set; }
    }
}
