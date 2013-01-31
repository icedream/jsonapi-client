using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonApi.Client.Classes
{
    public class MultipleAPIRequest : Collection<StandardAPIRequest>, IAPIRequest
    {
        public MultipleAPIRequest(string[] methodnames, object[][] argumentsarray)
        {
            if (methodnames.Length != argumentsarray.Length)
                throw new ArgumentException("Method names and arguments must have the same count.");

            this.AddRange(methodnames.Zip(argumentsarray, (methodname, arguments) => new StandardAPIRequest(methodname, arguments)));
        }

        public MultipleAPIRequest(IEnumerable<StandardAPIRequest> requests)
        {
            this.AddRange(requests);
        }

        public MultipleAPIRequest(params StandardAPIRequest[] requests)
        {
            this.AddRange(requests);
        }

        public void AddRange(StandardAPIRequest[] requests)
        {
            foreach (var r in requests)
                this.Add(r);
        }

        public void AddRange(IEnumerable<StandardAPIRequest> requests)
        {
            foreach (var r in requests)
                this.Add(r);
        }

        public string GenerateRequestString(Client client)
        {
            return string.Format("/api/call-multiple?method={0}&args={1}&key={2}", GetMethodsJson(), GetMethodArgsJson(), client.GetKey(GetMethodsJson()));
        }

        internal string[] GetMethods()
        {
            return this.Select(request => request.MethodName).ToArray();
        }

        internal object[][] GetMethodArgs()
        {
            return this.Select(request => request.Arguments).ToArray();
        }

        internal string GetMethodsJson()
        {
            return JsonConvert.SerializeObject(GetMethods());
        }

        internal string GetMethodArgsJson()
        {
            return JsonConvert.SerializeObject(GetMethodArgs());
        }
    }
}
