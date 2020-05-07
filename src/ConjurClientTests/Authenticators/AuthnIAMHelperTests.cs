using System;
using ConjurClient.Authenticators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConjurClientTests
{
    [TestClass]
    public class AuthnIAMHelperTests
    {
        private static DateTime date = new DateTime(1970, 1, 1, 1, 1, 1);
        private static String sessionToken = "thisIsMyToken";
        private static String signedHeaders = "host;x-amz-content-sha256;x-amz-date;x-amz-security-token";
        private static String payloadHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
        private String expectedCanonicalRequest = "GET\n/\nAction=GetCallerIdentity&Version=2011-06-15\nhost:sts.amazonaws.com\nx-amz-content-sha256:e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\nx-amz-date:19700101T010101Z\nx-amz-security-token:thisIsMyToken\n\nhost;x-amz-content-sha256;x-amz-date;x-amz-security-token\ne3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
        private static String expectedDateAsString = "19700101";
        private static String expectedAmzDateAsString = "19700101T010101Z";
        private static String expectedCreateStringToSign = $"AWS4-HMAC-SHA256\n{expectedAmzDateAsString}\n{expectedDateAsString}/us-east-1/sts/aws4_request\na4ec84e151d9cc55ae97ea6e575126535e49c284f651bc66275ed03818506189";
        private static String expectedAuthenticationRequest = "{\"host\": \"sts.amazonaws.com\", \"x-amz-date\": \"19700101T060101Z\", \"x-amz-security-token\": \"sessionToken\", \"x-amz-content-sha256\": \"e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855\", \"authorization\": \"AWS4-HMAC-SHA256 Credential=accessKey/19700101/us-east-1/sts/aws4_request, SignedHeaders=host;x-amz-content-sha256;x-amz-date;x-amz-security-token, Signature=9c9d03c90a55b460a7bb4d3caf4a2900518c7d51a4ad7e8f5aeadbd3050edaed\"}";

        [TestMethod]
        public void TestGetDate()
        {
            String dateAsString = AuthnIAMHelper.GetDate(date);
            Assert.AreEqual(expectedDateAsString, dateAsString);
        }

        [TestMethod]
        public void TestGetAmzDate()
        {
            String dateTimeAsString = AuthnIAMHelper.GetAmzDate(date);
            Assert.AreEqual(expectedAmzDateAsString, dateTimeAsString);
        }

        [TestMethod]
        public void TestSHA256Hash()
        {
            String expectedResults = "b94d27b9934d3e08a52e52d7da7dabfac484efe37a5380ee9088f7ace2efcde9";
            String result = AuthnIAMHelper.SHA256Hash("hello world");
            Assert.AreEqual(expectedResults, result);
        }

        [TestMethod]
        public void TestCreateStringToSign()
        {
            String result = AuthnIAMHelper.CreateStringToSign(expectedDateAsString, expectedAmzDateAsString, expectedCanonicalRequest);
            Assert.AreEqual(expectedCreateStringToSign, result);
        }

        [TestMethod]
        public void TestCreateCanonicalRequest()
        {
            String amzdate = AuthnIAMHelper.GetAmzDate(date);
            String receivedCanonicalRequest = AuthnIAMHelper.CreateCanonicalRequest(amzdate, sessionToken, signedHeaders, payloadHash);

            Assert.AreEqual(expectedCanonicalRequest, receivedCanonicalRequest);
        }

        [TestMethod]
        public void TestGetAuthenticationRequest()
        {
            String result = AuthnIAMHelper.GetAuthenticationRequest("accessKey", "secretKey", "sessionToken", date.ToUniversalTime());
            Assert.AreEqual(expectedAuthenticationRequest, result);
        }

        [TestMethod]
        public void TestGetAuthenticationRequestNow()
        {
            throw new Exception(AuthnIAMHelper.GetAuthenticationRequest("ASIAZB7BKVSOQNR53DK2", "c6ZelEi/Fo2YMtezw/pfbft/Oqnp6oX64PPQJUKW", "IQoJb3JpZ2luX2VjEEIaCXVzLWVhc3QtMSJGMEQCIFLWHwoJ6nGfJ7/WnAH256B9a62K52lNd9zpcRPs7RT+AiBNd3KQbkHABKvtIBk9W7sEH3NInIBIXGObhhMBqiB6tSq0Awh7EAEaDDYyMjcwNTk0NTc1NyIM7EY/PoE9pXlXzZXRKpEDHpFXbGUrnswgGeRtK2A1SI7aDFtaH+IKo77P1GZbJFmm55ZtxJpXMaxxznShj+i+DE5wNXoGmzrizcRT0s39p3uFK/9eYTYwePK5dscMZQiJpXpObces8XePtJ1YhqWfgioL9VhuQXCZqhdJrsN2kFAvGjAFpVdpnWwJDdUFaX4mBo2VWpU/C6a7oEKmjgnPiWJpykLNTFg2HTYc91rHXJac/t+QoNydPgyDiyV3EDSCuenartgzVxTKqt10nilnDXQ4Pq2x6/e9WL4451e3qbxIbxNZ88eeZBwM6gsqO0fd9zx2m81z2oeER/1tsWUwOYAcNGvqZgfW6L8z6+C+P5Slx9BT0BHCErYwpW9zJiN5C1NfeXdaxuUBr5DE4MAqlwHBLhNvK8CrOwF/WsoxbzyA03PbkDsJ9n8AwUhwm7qxvOWOpFh1U/YqGeypfUHGH0r7KwsfAS9P8vUh0O5mloFa26CpmmGDVc5GLoGYNx5SyjbqhmybEF1Rplrop9Nrx4tuvvVyt3tO+t1DwhxUnrUwnqHB9QU67AHQqyZ3JDgkr2HDbRLOWkozBjBcn4cfrA2N0dg++FChEoOHEhcYRJEWnKWD3zfo98ykb4dhUorU3jFsPqfdSGYE+L9LVkENvdNGDWKz5wGrnZC+bF01u4b6NaNl5QA5DQMgvmUgtpWaKkjP7SCU86b46stAGThRN6F/qA3bniA17DmNEZQ21b7OjR0HjofNA4ZPTQP09FSmTczRb8SPUJc0Des8KNxVju89yGxTt6s6a2dR5V11FZJPeSFV8vZWK9HtRd118A9E70PuPJ2rrlFXa34cPtMiOGky5BKixr45uvAvJJhXWhkkVsQAGA=="));
        }


    }
}


