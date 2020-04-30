using System;
using System.Net;
using ConjurClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConjurClientTests
{
    [TestClass]
    public class EndpointsTests
    {

        [TestMethod]
        public void TestEndpointsAuthenticate()
        {
            string baseAuthnUrl = TestConfig.ValidConfig.AuthnUrl.Replace(TestConfig.ValidConfig.ApplianceUrl, "");
            string expectedEndpoint = String.Format($"{baseAuthnUrl}/{TestConfig.ValidConfig.Account}/{WebUtility.UrlEncode(TestConfig.Username)}/authenticate");
            string receivedEndpoint = TestConfig.ValidEndpoints.Authenticate();

            Assert.AreEqual(receivedEndpoint, expectedEndpoint);
        }

        [TestMethod]
        public void TestEndpointsRetrieveSecret()
        {
            string variableId = "some/path/to/variable";

            string receivedEndpoint = TestConfig.ValidEndpoints.RetrieveSecret(variableId);
            string expectedEndpoint = $"/secrets/{TestConfig.Account}/variable/{WebUtility.UrlEncode(variableId)}";
             
            Assert.AreEqual(receivedEndpoint, expectedEndpoint);
        }

        [TestMethod]
        public void TestEndpointsAddSecret()
        {
            string variableId = "some/path/to/variable";

            string receivedEndpoint = TestConfig.ValidEndpoints.AddSecret(variableId);
            string expectedEndpoint = $"/secrets/{TestConfig.Account}/variable/{WebUtility.UrlEncode(variableId)}";

            Assert.AreEqual(receivedEndpoint, expectedEndpoint);
        }

        [TestMethod]
        public void TestEndpointsListResources()
        {
            string receivedEndpoint = TestConfig.ValidEndpoints.ListResources(null, null);
            string expectedEndpoint = $"/resources/{TestConfig.Account}";

            Assert.AreEqual(receivedEndpoint, expectedEndpoint);
        }

        [TestMethod]
        public void TestEndpointsListResourcesWithKind()
        {
            string kind = "variable";

            string receivedEndpoint = TestConfig.ValidEndpoints.ListResources(kind, null);
            string expectedEndpoint = $"/resources/{TestConfig.Account}/{kind}";

            Assert.AreEqual(receivedEndpoint, expectedEndpoint);
        }

        [TestMethod]
        public void TestEndpointsListResourcesWithSearch()
        {
            string search = "searchString";

            string receivedEndpoint = TestConfig.ValidEndpoints.ListResources(null, search);
            string expectedEndpoint = $"/resources/{TestConfig.Account}?search={search}";

            Assert.AreEqual(receivedEndpoint, expectedEndpoint);
        }

        [TestMethod]
        public void TestEndpointsListResourcesWithKindAndSearch()
        {
            string kind = "variable";
            string search = "searchString";

            string receivedEndpoint = TestConfig.ValidEndpoints.ListResources(kind, search);
            string expectedEndpoint = $"/resources/{TestConfig.Account}/{kind}?search={search}";

            Assert.AreEqual(receivedEndpoint, expectedEndpoint);
        }
    }
}


