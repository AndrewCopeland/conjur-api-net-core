using System.Net.Http;
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

        [TestMethod]
        public void TestValidConjurInfo()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, TestConfig.AuthnUrl, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            JObject info = conjur.GetInfo();
            string conjurAccount = info.SelectToken(".configuration.conjur.account").ToString();

            Assert.AreEqual(TestConfig.Account, conjurAccount);
        }

        [TestMethod]
        public void TestValidConjurHealth()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, TestConfig.AuthnUrl, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            JObject health = conjur.GetHealth();
            string uiService = health.SelectToken(".services.ui").ToString();

            Assert.AreEqual(uiService, "ok");
        }

        [TestMethod]
        public void TestValidConjurAuthenticate()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, TestConfig.AuthnUrl, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
        }

        [TestMethod]
        [ExpectedException(typeof(ConjurAuthorizationException))]
        public void TestInvalidConjurAuthenticate()
        {
            var invalidApiKey = Utilities.ToSecureString("invalidApiKey");
            var config = new Configuration(TestConfig.ApplianceUrl, TestConfig.AuthnUrl, TestConfig.Account, TestConfig.Username, invalidApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
        }

        [TestMethod]
        public void TestValidConjurRetrieveSecret()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, null, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
            conjur.RetrieveSecret("path/to/secret");
        }

        [TestMethod]
        [ExpectedException(typeof(ConjurResourceNotFoundException))]
        public void TestInvalidConjurRetrieveSecret()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, null, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
            conjur.RetrieveSecret("path/to/secret/not/real");
        }

        [TestMethod]
        [ExpectedException(typeof(ConjurAuthorizationException))]
        public void TestInvalidConjurRetrieveSecretApiKey()
        {
            var invalidApiKey = Utilities.ToSecureString("invalidApiKey");
            var config = new Configuration(TestConfig.ApplianceUrl, null, TestConfig.Account, TestConfig.Username, invalidApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
            conjur.RetrieveSecret("path/to/secret");
        }

        [TestMethod]
        public void TestConjurAddSecretValid()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, null, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
            string secretPath = "add/value/of/secret";
            conjur.AddSecret(secretPath, Utilities.ToSecureString("newSecret"));
            SecureString secretValue = conjur.RetrieveSecret(secretPath);
            Assert.AreEqual(Utilities.ToString(secretValue), "newSecret");
        }

        [TestMethod]
        [ExpectedException(typeof(ConjurResourceNotFoundException))]
        public void TestConjurAddSecretInvalidVariableId()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, null, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
            string secretPath = "add/value/of/secret/invalid";
            conjur.AddSecret(secretPath, Utilities.ToSecureString("newSecret"));
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public void TestConjurAddSecretInvalidSecretValue()
        {
            // return 422 when empty secret value
            var config = new Configuration(TestConfig.ApplianceUrl, null, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
            string secretPath = "add/value/of/secret";
            conjur.AddSecret(secretPath, Utilities.ToSecureString(""));
        }

        [TestMethod]
        public void TestConjurListResourcesValid()
        {
            var config = new Configuration(TestConfig.ApplianceUrl, null, TestConfig.Account, TestConfig.Username, TestConfig.ApiKey, true);
            Conjur conjur = new Conjur(config);
            conjur.Authenticate();
            conjur.ListResources();
        }
    }
}

