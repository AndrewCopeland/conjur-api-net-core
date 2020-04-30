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

        public SecureString RetrieveSecret(string variableId)
        {
            return Utilities.ToSecureString(_conjurService.RetrieveSecret(variableId));
        }

        public void AddSecret(string variableId, SecureString secretValue)
        {
            _conjurService.AddSecret(variableId, Utilities.ToString(secretValue));
        }

        public JArray ListResources()
        {
            return this.ListResources(null, null);
        }

        public JArray ListResources(string kind, string search)
        {
            return _conjurService.ListResources(kind, search);
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