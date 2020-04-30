using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using ConjurClient.Exceptions;
using Newtonsoft.Json.Linq;

namespace ConjurClient
{
    public class ConjurService
    {
        private Configuration _config { get; set; }
        private HttpClient _httpClient { get; set; }
        private Endpoints _endpoints { get; set; }
        private SecureString _authorizationToken { get; set; }

        public ConjurService(Configuration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
            _endpoints = new Endpoints(_config);
            _httpClient.BaseAddress = new Uri(_config.ApplianceUrl);
        }

        public ConjurService(Configuration config)
            : this(config, new HttpClient()) { }

        public JObject GetInfo()
        {
            String result = sendHttpRequest(_endpoints.Info());
            return Utilities.ToJObject(result);
        }

        public JObject GetHealth()
        {
            String result = sendHttpRequest(_endpoints.Health());
            return Utilities.ToJObject(result);
        }

        public SecureString Authenticate()
        {
            String endpoint = _endpoints.Authenticate();
            HttpMethod method = HttpMethod.Post;
            String body = Utilities.ToString(_config.ApiKey);

            String accessToken = sendHttpRequest(endpoint, method, false, body);

            return Utilities.ToSecureString(accessToken);
        }

        public String RetrieveSecret(string variableId)
        {
            String endpoint = _endpoints.RetrieveSecret(variableId);
            HttpMethod method = HttpMethod.Get;
            String body = null;

            String secretValue = sendHttpRequest(endpoint, method, true, body);

            return secretValue;
        }

        private String sendHttpRequest(String endpoint)
        {
            return sendHttpRequest(endpoint, HttpMethod.Get, false, null);
        }

        private String sendHttpRequest(String endpoint, HttpMethod method, Boolean accessToken, String data)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, endpoint);
            if (accessToken)
            {
                // _httpClient.DefaultRequestHeaders.Authorization = authorizationHeader;
                String base64AccessToken = Utilities.ToBase64String(Utilities.ToString(_config.AccessToken));
                String value = "Token token=\"" + base64AccessToken + "\"";
                // throw new Exception(value);
                _httpClient.DefaultRequestHeaders.Add("Authorization", value);
            }
            if (!String.IsNullOrWhiteSpace(data))
            {
                request.Content = new StringContent(data, Encoding.UTF8);
            }

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            String content = response.Content.ReadAsStringAsync().Result;

            // If 401 returned throw ConjurAuthorizationException
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new ConjurAuthorizationException("Unauthorized '401'. Invalid login, api key, or privileges.");
            }

            // If 404 returned throw ConjurResourceNotFoundException
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ConjurResourceNotFoundException("Not Found '404'. Invalid privileges, resource does not exists or secret value not populated.");

            }

            // If non-successful status code throw HttpRequestException
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP '{method}' '{_httpClient.BaseAddress}{endpoint}' returned status code '{(int)response.StatusCode}' with response of '{content}'");
            }

            return content;
        }

        private SecureString getLoginHeader()
        {
            String base64Creds = Utilities.ToBase64String(
                                    String.Format( "{0}:{1}",
                                        _config.Username, Utilities.ToString(_config.ApiKey)));

            String header = String.Format("Basic {0}", base64Creds);
            return Utilities.ToSecureString(header);
        }

        private AuthenticationHeaderValue GetAccessTokenHeader()
        {
            String base64AccessToken = Utilities.ToBase64String(Utilities.ToString(_config.AccessToken));
            return new AuthenticationHeaderValue("Token", String.Format("token=\"{0}\"", base64AccessToken));
        }


    }
}
