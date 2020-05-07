using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security;
using ConjurClient.Resources;
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


        /// <summary>This method will return conjur service information.
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    JObject info = c.GetInfo();
        /// </code>
        /// Result is conjur service info JObject
        /// </example>
        /// </summary>
        public JObject GetInfo()
        {
            return _conjurService.GetInfo();
        }

        /// <summary>This method will return conjur service health.
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    JObject health = c.GetHealth();
        /// </code>
        /// Result is conjur service health JObject
        /// </example>
        /// </summary>
        public JObject GetHealth()
        {
            return _conjurService.GetHealth();
        }

        /// <summary>This method will authenticate to conjur.
        ///     Conjur client access token is refreshed.
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    c.Authenticate();
        /// </code>
        /// </example>
        /// </summary>
        public void Authenticate()
        {
            _config.AccessToken = _conjurService.Authenticate();
        }

        /// <summary>This method will retrieve a secret from conjur
        ///     Secret is returned as a SecureString
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    SecureSecret password = c.RetrieveSecret("path/to/db/password");
        /// </code>
        /// Result is the secret as a SecureString
        /// </example>
        /// </summary>
        /// <param name="variableId">
        /// The variable id to the secret. e.g. path/to/db/password
        /// </param>
        public SecureString RetrieveSecret(string variableId)
        {
            return Utilities.ToSecureString(_conjurService.RetrieveSecret(variableId));
        }

        /// <summary>This method will add a secret value into conjur
        ///     Provided secret value must be a SecureString
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    SecureSecret password = c.AddSecret("path/to/db/password",
        ///                                 "newPassword");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="variableId">
        /// The variable id to the secret. e.g. path/to/db/password
        /// </param>
        /// <param name="secretValue">
        /// The new secret value.
        /// </param>
        public void AddSecret(string variableId, SecureString secretValue)
        {
            _conjurService.AddSecret(variableId, Utilities.ToString(secretValue));
        }

        /// <summary>This method will list all resources in conjur
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    JArray resources = c.ListResources();
        /// </code>
        /// </example>
        /// </summary>
        public List<Resource> ListResources()
        {
            return ListResources(null, null);
        }

        /// <summary>This method will list specific resources in conjur
        ///     depending on the kind and search query
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    JArray resources = c.ListResources();
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="kind">
        /// the kind of conjur resource. e.g. host, group, variable, etc
        /// </param>
        /// <param name="search">
        /// the search string.
        /// </param>
        public List<Resource> ListResources(string kind, string search)
        {
            JArray jsonResources = _conjurService.ListResources(kind, search);
            List<Resource> resources = new List<Resource>();
            foreach (JObject jsonResource in jsonResources)
            {
                resources.Add(new Resource(jsonResource));
            }
            return resources;
        }


        /// <summary>This method append a policy to a policy branch
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    JArray resources = c.AppendPolicy("root", "- !variable new");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="policyId">
        /// the policy branch policy is being loaded.
        /// </param>
        /// <param name="policyContent">
        /// the content of the policy yaml being loaded.
        /// </param>
        public JObject AppendPolicy(string policyId, string policyContent)
        {
            return _conjurService.AppendPolicy(policyId, policyContent);
        }

        /// <summary>This method replace a policy in a policy branch
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    JArray resources = c.ReplacePolicy("root", "- !variable new");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="policyId">
        /// the policy branch policy is being loaded.
        /// </param>
        /// <param name="policyContent">
        /// the content of the policy yaml being loaded.
        /// </param>
        public JObject ReplacePolicy(string policyId, string policyContent)
        {
            return _conjurService.ReplacePolicy(policyId, policyContent);
        }

        /// <summary>This method updates a policy in a policy branch
        /// <example>For example:
        /// <code>
        ///    Conjur c = new Conjur();
        ///    JArray resources = c.UpdatePolicy("root", "- !variable new");
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="policyId">
        /// the policy branch policy is being loaded.
        /// </param>
        /// <param name="policyContent">
        /// the content of the policy yaml being loaded.
        /// </param>
        public JObject UpdatePolicy(string policyId, string policyContent)
        {
            return _conjurService.UpdatePolicy(policyId, policyContent);
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