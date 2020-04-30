using System.IO;
using System.Net;
using System.Security;
using ConjurClient;
using ConjurClient.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace ConjurClientTests
{
    [TestClass]
    public class ConjurTests
    {
        string applianceUrl = "https://conjur-master";
        string authnUrl = "https://conjur-master/authn";
        string account = "conjur";
        string username = "admin";
        SecureString apiKey = Utilities.ToSecureString("35a9ej72v0q8ek25fghn52g1rjvm29qwxv738ts71j2d5hdwk1s34fbn");
        SecureString accessToken = Utilities.ToSecureString("superSecretAccessToken");
        static string invalidAccessTokenPath = @"../../../ConfigurationTests/invalid_access_token.txt";
        static string validAccessTokenPath = @"../../../ConfigurationTests/valid_access_token.txt";
        string content = File.ReadAllText(validAccessTokenPath);


        [TestMethod]
        public void TestValidConjurInfo()
        {
            var config = new Configuration(applianceUrl, authnUrl, account, username, apiKey, true);
            Conjur conjur = new Conjur(config);
            JObject info = conjur.GetInfo();
            string conjurAccount = info.SelectToken(".configuration.conjur.account").ToString();

            Assert.AreEqual(account, conjurAccount);
        }

        [TestMethod]
        public void TestValidConjurHealth()
        {
            var config = new Configuration(applianceUrl, authnUrl, account, username, apiKey, true);
            Conjur conjur = new Conjur(config);
            JObject health = conjur.GetHealth();
            string uiService = health.SelectToken(".services.ui").ToString();

            Assert.AreEqual(uiService, "ok");
        }

        [TestMethod]
        public void TestValidConjurAuthenticate()
        {
            var config = new Configuration(applianceUrl, authnUrl, account, username, apiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
        }

        [TestMethod]
        [ExpectedException(typeof(ConjurAuthorizationException))]
        public void TestInvalidConjurAuthenticate()
        {
            var invalidApiKey = Utilities.ToSecureString("invalidApiKey");
            var config = new Configuration(applianceUrl, authnUrl, account, username, invalidApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
        }

        [TestMethod]
        public void TestValidConjurRetrieveSecret()
        {
            var config = new Configuration(applianceUrl, null, account, username, apiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
            conjur.RetrieveSecret("path/to/secret");
        }
    }
}

