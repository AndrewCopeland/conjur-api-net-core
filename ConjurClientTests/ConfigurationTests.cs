using System;
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

        [TestMethod]
        public void TestConfigurationFullValid()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, TestConfig.AuthnUrl, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey);
            Assert.AreEqual(config.ApplianceUrl, TestConfig.ApplianceUrl);
            Assert.AreEqual(config.AuthnUrl, TestConfig.AuthnUrl);
            Assert.AreEqual(config.Account, TestConfig.Account);
            Assert.AreEqual(config.Username, TestConfig.Username);
            Assert.AreEqual(config.ApiKey, TestConfig.ApiKey);
        }

        [TestMethod]
        public void TestConfigurationNoAuthnUrlValid()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey);
            Assert.AreEqual(config.ApplianceUrl, TestConfig.ApplianceUrl);
            Assert.AreEqual<string>(config.AuthnUrl, TestConfig.AuthnUrl);
            Assert.AreEqual(config.Account, TestConfig.Account);
            Assert.AreEqual(config.Username, TestConfig.Username);
            Assert.AreEqual(config.ApiKey, TestConfig.ApiKey);
        }

        [TestMethod]
        public void TestConfigurationAccessTokenValid()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, TestConfig.Account, TestConfig.AccessToken);
            Assert.AreEqual(config.ApplianceUrl, TestConfig.ApplianceUrl);
            Assert.AreEqual(config.Account, TestConfig.Account);
            Assert.AreEqual(config.AccessToken, TestConfig.AccessToken);
        }

        [TestMethod]
        public void TestConfigurationAccessTokenAccessTokenPathContentValid()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, TestConfig.Account, TestConfig.ValidAccessTokenPath);
            Assert.AreEqual(config.ApplianceUrl, TestConfig.ApplianceUrl);
            Assert.AreEqual(config.Account, TestConfig.Account);
            Assert.AreEqual(config.AccessTokenPath, TestConfig.ValidAccessTokenPath);
            // comparing the password values as string instead of the actual secure string objects
            Assert.AreEqual(new NetworkCredential( "", config.AccessToken).Password,
                new NetworkCredential("", TestConfig.ValidAccessTokenContent).Password);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationApplianceUrlInvalid()
        {
            _ = new Configuration(null, TestConfig.AuthnUrl, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationAccountInvalid()
        {
            _ = new Configuration(TestConfig.ApplianceUrl, TestConfig.AuthnUrl, null, TestConfig.Username, TestConfig.ApiKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationUsernameInvalid()
        {
            _ = new Configuration(TestConfig.ApplianceUrl, TestConfig.AuthnUrl, TestConfig.Account, null, TestConfig.ApiKey);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationApiKeyInvalid()
        {
            _ = new Configuration(TestConfig.ApplianceUrl, TestConfig.AuthnUrl, TestConfig.Account, TestConfig.Username, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationAccessTokenApplianceUrlInvalid()
        {
            _ = new Configuration(null, TestConfig.Account, TestConfig.AccessToken);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationAccessTokenAccountInvalid()
        {
            _ = new Configuration(TestConfig.ApplianceUrl, null, TestConfig.AccessToken);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestConfigurationAccessTokenAccessTokenInvalid()
        {
            SecureString nullSecureString = null;
            _ = new Configuration(TestConfig.ApplianceUrl, TestConfig.Account, nullSecureString);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationAccessTokenAccessTokenPathInvalid()
        {
            _ = new Configuration(TestConfig.ApplianceUrl, TestConfig.Account, "");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationAccessTokenAccessTokenPathContentsInvalid()
        {
            _ = new Configuration(TestConfig.ApplianceUrl, TestConfig.Account, TestConfig.InvalidAccessTokenPath);
        }

        [TestMethod]
        public void TestConfigurationFromEnvironmentValidAuthnLogin()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", TestConfig.ApplianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", TestConfig.Username);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", Utilities.ToString(TestConfig.ApiKey));

            Configuration config = Configuration.FromEnvironment();
            Assert.AreEqual(config.ApplianceUrl, TestConfig.ApplianceUrl);
            Assert.AreEqual(config.Account, TestConfig.Account);
            Assert.AreEqual(config.Username, TestConfig.Username);
            Assert.AreEqual(Utilities.ToString(config.ApiKey), Utilities.ToString(TestConfig.ApiKey));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidApplianceUrl()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", "");
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", TestConfig.Username);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", Utilities.ToString(TestConfig.ApiKey));

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidAccount()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", "");
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", TestConfig.Username);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", new NetworkCredential("", TestConfig.ApiKey).Password);

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidUsername()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", "");
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", Utilities.ToString(TestConfig.ApiKey));

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidApiKey()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_LOGIN}", TestConfig.Username);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_API_KEY}", "");

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        public void TestConfigurationFromEnvironmentValidAccessToken()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", TestConfig.ApplianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN}", new NetworkCredential("", TestConfig.AccessToken).Password);

            Configuration config = Configuration.FromEnvironment();
            Assert.AreEqual(config.ApplianceUrl, TestConfig.ApplianceUrl);
            Assert.AreEqual(config.Account, TestConfig.Account);
            Assert.AreEqual(Utilities.ToString(config.AccessToken),
                Utilities.ToString(TestConfig.AccessToken));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidAccessToken()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", TestConfig.ApplianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN}", "");

            _ = Configuration.FromEnvironment();
        }

        [TestMethod]
        public void TestConfigurationFromEnvironmentValidAccessTokenPath()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", TestConfig.ApplianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN_FILE}", TestConfig.ValidAccessTokenPath);

            Configuration config = Configuration.FromEnvironment();
            Assert.AreEqual(config.ApplianceUrl, TestConfig.ApplianceUrl);
            Assert.AreEqual(config.Account, TestConfig.Account);
            Assert.AreEqual(config.AccessTokenPath, TestConfig.ValidAccessTokenPath);
            Assert.AreEqual(Utilities.ToString(config.AccessToken),
                TestConfig.ValidAccessTokenContent);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void TestConfigurationFromEnvironmentInvalidAccessTokenPath()
        {
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_APPLIANCE_URL}", TestConfig.ApplianceUrl);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_ACCOUNT}", TestConfig.Account);
            Environment.SetEnvironmentVariable($"{ConfigurationEnvironmentVariables.CONJUR_AUTHN_TOKEN_FILE}", TestConfig.InvalidAccessTokenPath);

            _ = Configuration.FromEnvironment();
        }
    }
}

