using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApi.Client.Classes
{
    public class StreamAPIRequest : IAPIRequest
    {
        public string SourceName { get; set; }
        public string Key { get; set; }
        public bool ShowPrevious { get; set; }
        public bool Unsubscribe { get; set; }

        public string GenerateRequestString(Client client)
        {
            return string.Format("/api/subscribe?source={0}&key={1}&show_previous={2}", SourceName, client.GetKey(SourceName), ShowPrevious.ToString().ToLower());
        }
    }
}
