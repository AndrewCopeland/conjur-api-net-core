using System;
using System.Net;

namespace ConjurClient
{
    public class Endpoints
    {
        private Configuration _config;

        public Endpoints(Configuration config)
        {
            _config = config;
        }

        public String Authenticate()
        {
            string baseAuthnUrl = _config.AuthnUrl.Replace(_config.ApplianceUrl, "");
            return String.Format($"{baseAuthnUrl}/{_config.Account}/{WebUtility.UrlEncode(_config.Username)}/authenticate");
        }

        public String RetrieveSecret(string variableId)
        {
            return String.Format($"/secrets/{_config.Account}/variable/{WebUtility.UrlEncode(variableId)}");
        }

        public String AddSecret(string variableId)
        {
            return String.Format($"/secrets/{_config.Account}/variable/{WebUtility.UrlEncode(variableId)}");
        }

        public String ListResources(string kind, string search)
        {
            string resourceEndpoint = String.Format($"/resources/{_config.Account}");
            if (!String.IsNullOrWhiteSpace(kind))
            {
                resourceEndpoint += $"/{kind}";
            }
            if (!String.IsNullOrWhiteSpace(search))
            {
                resourceEndpoint += $"?search={search}";
            }
            return resourceEndpoint;
        }

        public String LoadPolicy (string policyId)
        {
            policyId = WebUtility.UrlEncode(policyId);
            return $"/policies/{_config.Account}/policy/{policyId}";
        }

        public String Info()
        {
            return "/info";
        }

        public String Health()
        {
            return "/health";
        }
    }
}
