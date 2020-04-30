using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using Newtonsoft.Json.Linq;

namespace ConjurClient
{
    public class Conjur
    {
        private Configuration _config { get; set; }
        private ConjurService _conjurService { get; set; }

        public Conjur()
        {
            _config = Configuration.FromEnvironment();
            _conjurService = new ConjurService(_config, getDefaultHttpClient(_config));
        }

        public Conjur(Configuration config) : this (config, getDefaultHttpClient(config)) { }

        // this ctor will mainly be used for testing
        // since we can mock the httpClient
        public Conjur(Configuration config, HttpClient httpClient)
        {
            _config = config;
            _conjurService = new ConjurService(_config, httpClient);
        }


        public JObject GetInfo()
        {
            return _conjurService.GetInfo();
        }

        public JObject GetHealth()
        {
            return _conjurService.GetHealth();
        }

        public void Authenticate()
        {
            _config.AccessToken = _conjurService.Authenticate();
        }

        public String RetrieveSecret(string variableId)
        {
            return _conjurService.RetrieveSecret(variableId);
        }

        private static HttpClient getDefaultHttpClient(Configuration config)
        {
            if (config.IgnoreUntrustedSSL)
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                return new HttpClient(clientHandler);
            }
            return new HttpClient();
        }
    }
}