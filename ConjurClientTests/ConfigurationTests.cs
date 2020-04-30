using System;
using System.IO;
using System.Net;
using System.Security;
using ConjurClient;
using ConjurClient.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConjurClientTests
{
    [TestClass]
    public class ConfigurationTests
    {
        string applianceUrl = "https://conjur-master";
        string authnUrl = "https://conjur-master/authn";
        string account = "account";
        string username = "host/test";
        SecureString apiKey = new NetworkCredential("", "superSecretApiKey").SecurePassword;
        SecureString accessToken = new NetworkCredential("", "superSecretAccessToken").SecurePassword;
        static string invalidAccessTokenPath = @"../../../ConfigurationTests/invalid_access_token.txt";
        static string validAccessTokenPath = @"../../../ConfigurationTests/valid_access_token.txt";
        string content = File.ReadAllText(validAccessTokenPath);

        [TestMethod]
        public void TestConfigurationFullValid()
        {
            var config = new Configuration(applianceUrl, authnUrl, account, username, apiKey);
            Assert.AreEqual(config.ApplianceUrl, applianceUrl);
            Assert.AreEqual(config.AuthnUrl, authnUrl);
            Assert.AreEqual(config.Account, account);
            Assert.AreEqual(config.Username, username);
            Assert.AreEqual(config.ApiKey, apiKey);
        }

        [TestMethod]
        public void TestConfigurationNoAuthnUrlValid()
        {
            var config = new Configuration(applianceUrl, account, username, apiKey);
            Assert.AreEqual(config.ApplianceUrl, applianceUrl);
            Assert.AreEqual<string>(config.AuthnUrl, authnUrl);
            Assert.AreEqual(config.Account, account);
            Assert.AreEqual(config.Username, username);
            Assert.AreEqual(config.ApiKey, apiKey);
        }

        [TestMethod]
        public void TestConfigurationAccessTokenValid()
        {
            var config = new Configuration(applianceUrl, account, accessToken);
            Assert.AreEqual(config.ApplianceUrl, applianceUrl);
            Assert.AreEqual(config.Account, account);
            Assert.AreEqual(config.AccessToken, accessToken);
        }

        [TestMethod]
        public void TestConfigurationAccessTokenAccessTokenPathContentValid()
        {
            var config = new Configuration(applianceUrl, account, validAccessTokenPath);
            Assert.AreEqual(config.ApplianceUrl, applianceUrl);
            Assert.AreEqual(config.Account, account);
            Assert.AreEqual(config.AccessTokenPath, validAccessTokenPath);
            // comparing the password values as string instead of the actual secure string objects
            Assert.AreEqual(new NetworkCredential( "", config.AccessToken).Password,
                new NetworkCredential("", content).Password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationApplianceUrlInvalid()
        {
            _ = new Configuration(null, authnUrl, account, username, apiKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationAccountInvalid()
        {
            _ = new Configuration(applianceUrl, authnUrl, null, username, apiKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationUsernameInvalid()
        {
            _ = new Configuration(applianceUrl, authnUrl, account, null, apiKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationApiKeyInvalid()
        {
            _ = new Configuration(applianceUrl, authnUrl, account, username, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationAccessTokenApplianceUrlInvalid()
        {
            _ = new Configuration(null, account, accessToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationAccessTokenAccountInvalid()
        {
            _ = new Configuration(applianceUrl, null, accessToken);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationAccessTokenAccessTokenInvalid()
        {
            SecureString nullSecureString = null;
            _ = new Configuration(applianceUrl, account, nullSecureString);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationAccessTokenAccessTokenPathInvalid()
        {
            _ = new Configuration(applianceUrl, account, "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationAccessTokenAccessTokenPathContentsInvalid()
        {
            _ = new Configuration(applianceUrl, account, invalidAccessTokenPath);
        }

        [TestMethod]
        public void TestConfigurationFromEnvironmentValidAuthnLogin()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", applianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", username);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", new NetworkCredential("", apiKey).Password);

            Configuration config = Configuration.FromEnvironment();
            Assert.AreEqual(config.ApplianceUrl, applianceUrl);
            Assert.AreEqual(config.Account, account);
            Assert.AreEqual(config.Username, username);
            Assert.AreEqual(new NetworkCredential("", config.ApiKey).Password,
                new NetworkCredential("", apiKey).Password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidApplianceUrl()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", "");
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", username);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", new NetworkCredential("", apiKey).Password);

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidAccount()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", "");
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", username);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", new NetworkCredential("", apiKey).Password);

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidUsername()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", "");
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", new NetworkCredential("", apiKey).Password);

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidApiKey()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", username);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", "");

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        public void TestConfigurationFromEnvironmentValidAccessToken()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", applianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN}", new NetworkCredential("", accessToken).Password);

            Configuration config = Configuration.FromEnvironment();
            Assert.AreEqual(config.ApplianceUrl, applianceUrl);
            Assert.AreEqual(config.Account, account);
            Assert.AreEqual(new NetworkCredential("", config.AccessToken).Password,
                new NetworkCredential("", accessToken).Password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidAccessToken()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", applianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN}", "");

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        public void TestConfigurationFromEnvironmentValidAccessTokenPath()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", applianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN_FILE}", validAccessTokenPath);

            Configuration config = Configuration.FromEnvironment();
            Assert.AreEqual(config.ApplianceUrl, applianceUrl);
            Assert.AreEqual(config.Account, account);
            Assert.AreEqual(config.AccessTokenPath, validAccessTokenPath);
            Assert.AreEqual(new NetworkCredential("", config.AccessToken).Password,
                new NetworkCredential("", content).Password);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidAccessTokenPath()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", applianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN_FILE}", invalidAccessTokenPath);

            _ = Configuration.FromEnvironment();
        }
    }
}

