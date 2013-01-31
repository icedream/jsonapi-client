using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApi.Client.Classes
{
    public interface IAPIRequest
    {
        string GenerateRequestString(Client client);
    }
}
