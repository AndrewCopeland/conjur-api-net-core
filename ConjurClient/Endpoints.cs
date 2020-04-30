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
            string baseAuthnUrl = _config.AuthnUrl.Replace(_config.ApplianceUrl + "/", "") + "/" + _config.Account;
            return String.Format($"{baseAuthnUrl}/{_config.Account}/{WebUtility.UrlEncode(_config.Username)}/authenticate");
        }

        public String RetrieveSecret(string variableId)
        {
            return String.Format($"secrets/{_config.Account}/variable/{WebUtility.UrlEncode(variableId)}");
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
