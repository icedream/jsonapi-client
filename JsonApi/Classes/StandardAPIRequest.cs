using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonApi.Client.Classes
{
    public class StandardAPIRequest : IAPIRequest
    {
        // TODO: Proper stream response handline
        // TODO: Split synchronous requests to HTTP or seperate TCP socket

        public string MethodName { get; set; }
        public object[] Arguments { get; set; }
        public string Identifier { get; set; }

        public string GenerateRequestString(Client client)
        {
            return string.Format(
                "/api/call?method={0}&args={1}&key={3}&identifier={2}",
                Uri.EscapeDataString(MethodName),
                Uri.EscapeDataString(GetArgumentsJson()),
                !string.IsNullOrEmpty(Identifier) ? Uri.EscapeDataString(Identifier) : null,
                client.GetKey(MethodName)
            );
        }

        internal string GetArgumentsJson()
        {
            return JsonConvert.SerializeObject(Arguments != null ? Arguments : new object[] { });
        }

        public StandardAPIRequest(string method)
        {
            MethodName = method;
            Arguments = new object[0];
            Identifier = null;
        }
        public StandardAPIRequest(string method, params object[] arguments)
        {
            MethodName = method;
            Arguments = arguments;
            Identifier = null;
        }
        /*
        public StandardAPIRequest(string identifier, string method, params object[] arguments)
        {
            MethodName = method;
            Arguments = arguments;
            Identifier = identifier;
        }
         */
    }
}
