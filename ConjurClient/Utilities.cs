using System;
using System.Net;
using System.Security;
using Newtonsoft.Json.Linq;

namespace ConjurClient
{
    public static class Utilities
    {
        public static String ToString(SecureString cred)
        {
            return new NetworkCredential("", cred).Password;
        }

        public static SecureString ToSecureString(String password)
        {
            return new NetworkCredential("", password).SecurePassword;
        }

        public static String ToBase64String(String content)
        {
            return Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes(content));
        }

        public static JObject ToJObject(string content)
        {
            return JObject.Parse(content);
        }
    }
}
