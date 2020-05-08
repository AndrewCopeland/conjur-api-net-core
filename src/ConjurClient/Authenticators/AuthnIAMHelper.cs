using System;
using System.Security.Cryptography;
using System.Text;

namespace ConjurClient.Authenticators
{
    public static class AuthnIAMHelper
    {
        public static String HOST = "sts.amazonaws.com";
        public static String REGION = "us-east-1";
        public static String SERVICE = "sts";
        public static String SIGNED_HEADERS = "host;x-amz-content-sha256;x-amz-date;x-amz-security-token";

        public static String GetAmzDate(DateTime now)
        {
            String date = GetDate(now);
            String time = now.ToString("HHmmss");

            return date + "T" + time + "Z";
        }

        public static String GetDate(DateTime now)
        {
            return now.ToString("yyyyMMdd");
        }

        public static String CreateCanonicalRequest(String amzdate, String token, String signedHeaders, String payloadHash)
        {
            String canonicalUri = "/";
            String canonicalQueryString = "Action=GetCallerIdentity&Version=2011-06-15";
            String canonicalHeaders = "host:" + HOST + "\n" + "x-amz-content-sha256:" + payloadHash + "\n" + "x-amz-date:" + amzdate + "\n" + "x-amz-security-token:" + token + "\n";
            String canonicalRequest = "GET" + "\n" + canonicalUri + "\n" + canonicalQueryString + "\n" + canonicalHeaders + "\n" + signedHeaders + "\n" + payloadHash;
            return canonicalRequest;
        }

        public static String GetCredentialScope(String datestamp)
        {
            return datestamp + '/' + REGION + '/' + SERVICE + '/' + "aws4_request";
        }

        public static String SHA256Hash(String input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static byte[] HMACSHA256Hash(String data, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                byte[] bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return bytes;
            }
        }

        public static String CreateStringToSign(String datestamp, String amzdate, String cannonicalRequest)
        {
            String algorithm = "AWS4-HMAC-SHA256";
            String credentialScope = GetCredentialScope(datestamp);
            String stringToSign = algorithm + "\n" + amzdate + "\n" + credentialScope + "\n" + SHA256Hash(cannonicalRequest);
            return stringToSign;
        }

        public static byte[] GetSignatureKey(String key, String dateStamp, String regionName, String serviceName)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes("AWS4" + key);
            byte[] kDate = HMACSHA256Hash(dateStamp, kSecret);
            byte[] kRegion = HMACSHA256Hash(regionName, kDate);
            byte[] kService = HMACSHA256Hash(serviceName, kRegion);
            byte[] kSigning = HMACSHA256Hash("aws4_request", kService);
	        return kSigning;
	    }

        public static String SignString(String stringToSign, byte[] signingKey)
        {
		    byte[] hash = HMACSHA256Hash(stringToSign, signingKey);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static String GetAuthorizationHeader(String accessKey, String credentialScope, String signedHeaders, String signature)
        {
            String algorithm = "AWS4-HMAC-SHA256";
            return String.Format("{0} Credential={1}/{2}, SignedHeaders={3}, Signature={4}", algorithm, accessKey, credentialScope, signedHeaders, signature);
            // return algorithm + " " + "Credential=" + accessKey + '/' + credentialScope + ", " + "SignedHeaders=" + signedHeaders + ", " + "Signature=" + signature;
        }

        public static String HeaderAsJsonString(String amzdate, String token, String payloadHash, String authorizationHeader)
        {
            String headerTemplate = "{{\"host\": \"{0}\", \"x-amz-date\": \"{1}\", \"x-amz-security-token\": \"{2}\", \"x-amz-content-sha256\": \"{3}\", \"authorization\": \"{4}\"}}";
            return String.Format(headerTemplate, HOST, amzdate, token, payloadHash, authorizationHeader);

        }

        // Is used for unit tests
        public static SecureString GetAuthenticationRequest(String accessKey, String secretKey, String sessionToken, DateTime now)
        {
            String amzdate = GetAmzDate(now);
            String datestamp = GetDate(now);

            String signedHeaders = "host;x-amz-content-sha256;x-amz-date;x-amz-security-token";
            // payload is empty hence the hardcoded hash
            String payloadHash = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";

            String canonicalRequest = CreateCanonicalRequest(amzdate, sessionToken, signedHeaders, payloadHash);
            String stringToSign = CreateStringToSign(datestamp, amzdate, canonicalRequest);
            String signature = "";

            byte[] signingKey = GetSignatureKey(secretKey, datestamp, REGION, SERVICE);
            signature = SignString(stringToSign, signingKey);
            String authorizationHeader = GetAuthorizationHeader(accessKey, GetCredentialScope(datestamp), SIGNED_HEADERS, signature);
            return Utilities.AsSecureString(HeaderAsJsonString(amzdate, sessionToken, payloadHash, authorizationHeader));
        }

        public static SecureString GetAuthenticationRequest(String accessKey, String secretKey, String sessionToken)
        {
            return GetAuthenticationRequest(accessKey, secretKey, sessionToken, DateTime.UtcNow);
        }

    }
}
