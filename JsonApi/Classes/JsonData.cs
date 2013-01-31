using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApi.Client.Classes
{
    internal class JsonDataEventArgs : EventArgs
    {
        public JObject Data { get; private set; }

        internal JsonDataEventArgs(JObject data)
        {
            Data = data;
        }

        internal JsonDataEventArgs(string jsonData)
        {
            Data = JObject.Parse(jsonData);
        }
    }
}
