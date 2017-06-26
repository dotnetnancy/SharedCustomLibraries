using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetNancy.GeneralApps.WebApi.Common
{
    public static class UriHelper
    {
        public static Uri AttachParameters(this Uri uri, NameValueCollection parameters)
        {
            var stringBuilder = new StringBuilder();
            string str = "?";
            for (int index = 0; index < parameters.Count; ++index)
            {
                stringBuilder.Append(str + parameters.AllKeys[index] + "=" + parameters[index]);
                str = "&";
            }
            return new Uri(uri + stringBuilder.ToString());
        }
    }
}
