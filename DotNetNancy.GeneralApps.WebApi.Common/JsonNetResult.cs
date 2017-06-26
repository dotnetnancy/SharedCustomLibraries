using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Newtonsoft.Json;

namespace DotNetNancy.GeneralApps.WebApi.Common
{
    public class JsonNetResult : ActionResult
    {
        [JsonIgnore]
        public Encoding ContentEncoding { get; set; }
        [JsonIgnore]
        public string ContentType { get; set; }
        public object Data { get; set; }
        public bool DomainSuccess { get; set; }
        public object ID { get; set; }
        public List<JsonErrorModel> DomainErrors { get; set; }
        [JsonIgnore]
        public JsonSerializerSettings SerializerSettings { get; set; }
        [JsonIgnore]
        public Newtonsoft.Json.Formatting Formatting { get; set; }
        public PaginationInfo PaginationInfo { get; set; }


        public JsonNetResult()
        {
            SerializerSettings = new JsonSerializerSettings();
            //initialize it to true, only set to false if there are domain errors, otherwise there would be 
            //http codes at a higher level in the stack 404 error 200 OK etc
            DomainSuccess = true;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType)
              ? ContentType
              : "application/json";

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

                JsonSerializer serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }
    }

}
