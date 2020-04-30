using System;
using System.IO;
using System.Security;
using ConjurClient;

namespace ConjurClientTests
{
    public static class TestConfig {
        public static string ApplianceUrl = "https://conjur-master";
        public static string AuthnUrl = "https://conjur-master/authn";
        public static string Account = "conjur";
        public static string Username = "admin";
        public static SecureString ApiKey = Utilities.ToSecureString("35a9ej72v0q8ek25fghn52g1rjvm29qwxv738ts71j2d5hdwk1s34fbn");
        public static SecureString AccessToken = Utilities.ToSecureString("superSecretAccessToken");
        public static string InvalidAccessTokenPath = String.Format("..{0}..{0}..{0}ConfigurationTests{0}invalid_access_token.txt", Path.DirectorySeparatorChar);
        public static string ValidAccessTokenPath = String.Format("..{0}..{0}..{0}ConfigurationTests{0}valid_access_token.txt", Path.DirectorySeparatorChar);
        public static string ValidAccessTokenContent = File.ReadAllText(ValidAccessTokenPath);
    }
}
