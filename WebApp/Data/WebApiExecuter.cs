using Newtonsoft.Json;
using System.Text.Json;
using System.Net.Http.Headers;

namespace WebApp.Data
{
    public class WebApiExecuter : IWebApiExecuter
    {
        private const string apiName = "ShirtsApi";
        private const string authApiName = "AuthorityApi";
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public WebApiExecuter(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        // method for http GET 

        /*
         * httpClientFactory.CreateClient(apiName):
httpClientFactory is an instance of IHttpClientFactory, which is a factory class provided by ASP.NET Core for creating and managing instances of HttpClient.
CreateClient(apiName) is a method of IHttpClientFactory used to create an instance of HttpClient.
apiName is a string parameter representing the name of the HTTP client to create. This name corresponds to the named HTTP client registration in the application's service collection.
This method retrieves a pre-configured HttpClient instance from the factory based on the specified client name (apiName).
         */
        public async Task<T?> InvokeGet<T>(string relativeUrl)
        {                           
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);

            //return await httpClient.GetFromJsonAsync<T>(relativeUrl);
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            var response = await httpClient.SendAsync(request);
            await HandlePotentialError(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        /*
         * httpClient.PostAsJsonAsync(relativeUrl, obj):
httpClient is an instance of HttpClient created using httpClientFactory.
PostAsJsonAsync(relativeUrl, obj) is a method of HttpClient used to send an HTTP POST request to the specified URL (relativeUrl) with a JSON payload (obj).
relativeUrl is a string parameter representing the relative URL of the API endpoint to which the POST request will be sent.
obj is an object that represents the payload to be sent with the POST request. It will be serialized to JSON format and included in the request body.
This method returns a Task<HttpResponseMessage> representing the asynchronous operation of sending the POST request. The HttpResponseMessage contains the HTTP response from the server.
         */
        //this is for create
        public async Task<T?> InvokePost<T>(string relativeUrl, T obj)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            var response = await httpClient.PostAsJsonAsync(relativeUrl, obj);
            //response.EnsureSuccessStatusCode();

            //if(!response.IsSuccessStatusCode)
            //{
            //    var errorJson = await response.Content.ReadAsStringAsync();
            //  //  var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorJson);
            //  throw new WebApiException(errorJson);
            //}

            await HandlePotentialError(response);

            return await response.Content.ReadFromJsonAsync<T>();
        }

        // this is for update
        public async Task InvokePut<T>(string relativeUrl, T obj)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            var response = await httpClient.PutAsJsonAsync(relativeUrl, obj);
            //response.EnsureSuccessStatusCode();
            await HandlePotentialError(response);
        }

        // this is for delete
        public async Task InvokeDelete(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(apiName);
            await AddJwtToHeader(httpClient);
            var response = await httpClient.DeleteAsync(relativeUrl);
            //response.EnsureSuccessStatusCode();
            await HandlePotentialError(response);
        }

        private async Task HandlePotentialError(HttpResponseMessage httpResponse)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorJson = await httpResponse.Content.ReadAsStringAsync();
                throw new WebApiException(errorJson);
            }
        }

        private async Task AddJwtToHeader(HttpClient httpClient)
        {
            JwtToken? token = null;
            string? strToken = httpContextAccessor.HttpContext?.Session.GetString("access_token");
            if (!string.IsNullOrWhiteSpace(strToken))
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strToken);
            }

            if(token == null || token.ExpiresAt <= DateTime.UtcNow)
            {
                var clientId = configuration.GetValue<string>("ClientId");
                var secret = configuration.GetValue<string>("Secret");

                // Authenticate against the authority: for authentication we will be needing the app credential class 
                var authoClient = httpClientFactory.CreateClient(authApiName);
                var response = await authoClient.PostAsJsonAsync("auth", new AppCredential
                {
                    ClientId = clientId,
                    Secret = secret
                });
                response.EnsureSuccessStatusCode();


                // Get the JWT from the authority: for getting the JWT we will need a class that represents the structure of the JWT that is returned
                strToken = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<JwtToken>(strToken);

                httpContextAccessor.HttpContext?.Session.SetString("access_token", strToken);
            }
            // Pass the JWT to endpoints through the http headers
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken);

        }
    }
}
