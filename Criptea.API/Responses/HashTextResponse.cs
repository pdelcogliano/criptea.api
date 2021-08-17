using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Criptea.API.Responses
{
    public class HashTextResponse
    {
        public string HashedText { get; set; }

        public string Salt { get; set; }
    }
}
