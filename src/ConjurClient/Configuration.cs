using System;
using System.IO;
using System.Net;
using System.Security;
using ConjurClient.Exceptions;

namespace ConjurClient
{
    public class Configuration
    {
        public String ApplianceUrl { get; set; }
        public String AuthnUrl { get; set; }
        public String Account { get; set; }
        public String Username { get; set; }
        public SecureString ApiKey { get; set; }
        public SecureString AccessToken { get; set; }
        public String AccessTokenPath { get; set; }
        public Boolean IgnoreUntrustedSSL { get; set; }

        // For api key and alternative authentication methods using untrusted cert
        public Configuration(String applianceUrl, String authnUrl, String account, String username, SecureString apiKey, Boolean ignoreUntrustedSSL)
        {
            // validate required arguments
            _ = applianceUrl ?? throw new ArgumentNullException(nameof(applianceUrl));
            _ = account ?? throw new ArgumentNullException(nameof(account));
            _ = username ?? throw new ArgumentNullException(nameof(username));
            _ = apiKey ?? throw new ArgumentNullException(nameof(apiKey));

            // default to api key authentication
            if (authnUrl == null)
            {
                authnUrl = applianceUrl + "/authn";
            }

            ApplianceUrl = applianceUrl;
            AuthnUrl = authnUrl;
            Account = account;
            Username = username;
            ApiKey = apiKey;
            AccessToken = null;
            IgnoreUntrustedSSL = ignoreUntrustedSSL;
        }

        // For api key and alternative authentication methods using trusted cert
        public Configuration(String applianceUrl, String authnUrl, String account, String username, SecureString apiKey)
            : this(applianceUrl, authnUrl, account, username, apiKey, false) { }

        // For api key authentication using trusted cert
        public Configuration(String applianceUrl, String account, String username, SecureString apiKey)
            : this(applianceUrl, null, account, username, apiKey, false) { }

        // For api key authentication using untrusted ssl cert
        public Configuration(String applianceUrl, String account, String username, SecureString apiKey, Boolean ignoreUntrustedSSL)
            : this(applianceUrl, null, account, username, apiKey, ignoreUntrustedSSL) { }

        // For access token authentication from SecureString using untrusted cert
        public Configuration(String applianceUrl, String account, SecureString accessToken, Boolean ignoreUntrustedSSL)
        {
            // validate required arguments
            _ = applianceUrl ?? throw new ArgumentNullException(nameof(applianceUrl));
            _ = account ?? throw new ArgumentNullException(nameof(account));
            _ = accessToken ?? throw new ArgumentNullException(nameof(accessToken));

            ApplianceUrl = applianceUrl;
            AuthnUrl = null;
            Account = account;
            Username = null;
            ApiKey = null;
            AccessToken = accessToken;
            IgnoreUntrustedSSL = ignoreUntrustedSSL;
        }

        // For access token authentication from SecureString using trusted cert
        public Configuration(String applianceUrl, String account, SecureString accessToken)
            : this(applianceUrl, account, accessToken, false) { }

        // For access token authentication from a token file using untrusted cert
        public Configuration(String applianceUrl, String account, String accessTokenPath, Boolean ignoreUntrustedSSL)
        {
            // validate required arguments
            _ = applianceUrl ?? throw new ArgumentNullException(nameof(applianceUrl));
            _ = account ?? throw new ArgumentNullException(nameof(account));
            _ = accessTokenPath ?? throw new ArgumentNullException(nameof(accessTokenPath));

            // open the access token file and init AccessToken
            String accessToken = null;
            try
            {
                accessToken = File.ReadAllText(accessTokenPath);
            }
            catch (IOException ex)
            {
                throw new InvalidConfigurationException(String.Format("Failed to read access token from file '{0}'", accessTokenPath), ex);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is ArgumentNullException)
                {
                    throw new InvalidConfigurationException(String.Format("Invalid argument '{0}'", nameof(accessTokenPath)), ex);
                }

                if (ex is PathTooLongException || ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    throw new InvalidConfigurationException(String.Format("Access token file '{0}' could not be found", accessTokenPath), ex);
                }

                if (ex is UnauthorizedAccessException || ex is SecurityException)
                {
                    throw new InvalidConfigurationException(String.Format("Invalid permissions to read access token file '{0}'", accessTokenPath), ex);
                }

                throw;
            }

            if (String.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidConfigurationException(String.Format("Invalid access token. Validate contents of '{0}'", accessTokenPath));
            }

            // convert access token into a secure string
            SecureString accessTokenSecure = Utilities.ToSecureString(accessToken);
            // clear access token as string
            accessToken = null;

            ApplianceUrl = applianceUrl;
            AuthnUrl = null;
            Account = account;
            Username = null;
            ApiKey = null;
            AccessToken = accessTokenSecure;
            AccessTokenPath = accessTokenPath;
            IgnoreUntrustedSSL = ignoreUntrustedSSL;
        }

        // For access token authentication from a token file using trusted cert
        public Configuration(String applianceUrl, String account, String accessTokenPath)
            : this(applianceUrl, account, accessTokenPath, false) { }

        // Generate a configuration from environment variables available to this appliacation
        public static Configuration FromEnvironment()
        {
            // required environment variables
            String applianceUrl = getRequiredEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}");
            String account = getRequiredEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}");

            // access token related environment variables
            String accessToken = getEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN}");
            String accessTokenPath = getEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN_FILE}");

            // ignoring ssl
            String ignoreUntrustedSSL = Environment.GetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_IGNORE_UNTRUSTED_SSL}");
            Boolean ignoreSSL = false;
            if (ignoreUntrustedSSL != null &&
                (ignoreUntrustedSSL.ToLower() == "true" || ignoreUntrustedSSL.ToLower() == "yes"))
            {
                ignoreSSL = true;
            }

            // Check if access token or access token path is being used
            if (accessToken != null)
            {
                SecureString accessTokenSecure = Utilities.ToSecureString(accessToken);
                accessToken = null;
                return new Configuration(applianceUrl, account, accessTokenSecure, ignoreSSL);
            }
            if (accessTokenPath != null)
            {
                return new Configuration(applianceUrl, account, accessTokenPath, ignoreSSL);
            }

            // since not using an access token
            // assume api key or alterntive authentication method
            String authnLogin = getRequiredEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}");
            String apiKey = getRequiredEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}");
            String authnUrl = getEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_URL}");
            SecureString apiKeySecure = Utilities.ToSecureString(apiKey);
            apiKey = null;

            return new Configuration(applianceUrl, authnUrl, account, authnLogin, apiKeySecure, ignoreSSL);
        }

        private static String getEnvironmentVariable(String key)
        {
            return getEnvironmentVariable(key, null);
        }

        private static String getEnvironmentVariable(String key, String def)
        {
            String value = Environment.GetEnvironmentVariable(key);
            if (value == null)
            {
                value = def;
            }
            return value;
        }

        private static String getRequiredEnvironmentVariable(String key)
        {
            String value = getEnvironmentVariable(key, null);
            if (value == null)
            {
                throw new InvalidConfigurationException(String.Format("Failed to retrieve required environment variable '{0}'", key));
            }
            return value;
        }

    }
}
