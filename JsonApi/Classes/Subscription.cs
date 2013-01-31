using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonApi.Client.Classes
{
    public class Subscription
    {
        public string Source { get; internal set; }

        public event EventHandler<StreamMessageEventArgs> Receive;

        internal void Pass(StreamMessageEventArgs e)
        {
            if(Receive != null)
                Receive.Invoke(this, e);
        }
    }
}
