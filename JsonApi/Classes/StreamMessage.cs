using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonApi.Client.Classes
{
    public class StreamMessage
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("success")]
        public Dictionary<string, object> Data { get; set; }
    }

    public class StreamMessageEventArgs : EventArgs
    {
        public StreamMessage Message { get; private set; }

        public StreamMessageEventArgs(StreamMessage msg)
        {
            this.Message = msg;
        }
    }
}
