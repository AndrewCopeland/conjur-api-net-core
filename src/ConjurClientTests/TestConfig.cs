using System;
using System.IO;
using System.Security;
using ConjurClient;

namespace ConjurClientTests
{
    public static class TestConfig {
        public static string ApplianceUrl = FromEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}");
        public static string AuthnUrl = FromEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_URL}");
        public static string Account = FromEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}");
        public static string Username = FromEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}");
        public static SecureString ApiKey = Utilities.ToSecureString(FromEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}"));
        public static SecureString AccessToken = Utilities.ToSecureString("superSecretAccessToken");
        public static string InvalidAccessTokenPath = String.Format("..{0}..{0}..{0}ConfigurationTests{0}invalid_access_token.txt", Path.DirectorySeparatorChar);
        public static string ValidAccessTokenPath = String.Format("..{0}..{0}..{0}ConfigurationTests{0}valid_access_token.txt", Path.DirectorySeparatorChar);
        public static string ValidAccessTokenContent = File.ReadAllText(ValidAccessTokenPath);
        public static Configuration ValidConfig = new Configuration(ApplianceUrl, AuthnUrl, Account, Username, ApiKey, true);
        public static Endpoints ValidEndpoints = new Endpoints(ValidConfig);
        public static string ValidPolicyId = "root";
        public static string ValidPolicyContent = "- !variable path/to/secret \n- !variable add/value/of/secret";

        public static string FromEnvironmentVariable(string key)
        {
            string value = Environment.GetEnvironmentVariable(key);
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new Exception(String.Format("Failed to find environment variable {0}", key));
            }
            return value;
        }
    }
}
