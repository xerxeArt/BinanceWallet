using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Configuration
{
    public class BinanceConfig
    {
        public string Scheme { get; set; }
        public string BaseUrl { get; set; }
        public int Port { get; set; }
        public int Timeout { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }
}
