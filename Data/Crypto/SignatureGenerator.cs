using Data.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Data.Crypto
{
    public class SignatureGenerator : ISignatureGenerator
    {
        private readonly BinanceConfig _config;

        public SignatureGenerator(BinanceConfig config)
        {
            _config = config;
        }

        public string GenerateHMACSHA256(string textToHash)
        {
            using (var x = new HMACSHA256(Encoding.ASCII.GetBytes(_config.SecretKey)))
            {
                x.Initialize();
                var hash = x.ComputeHash(Encoding.ASCII.GetBytes(textToHash));

                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
