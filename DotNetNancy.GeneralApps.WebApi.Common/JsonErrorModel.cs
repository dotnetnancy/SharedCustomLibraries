using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNancy.GeneralApps.WebApi.Common
{
    public class JsonErrorModel
    {
        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
