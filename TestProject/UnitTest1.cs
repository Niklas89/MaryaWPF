using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace TestProject
{
    public class UnitTest1
    {
        private HttpClient _apiClient;
        // private readonly ITestOutputHelper _output;

        public HttpClient ApiClient
        {
            get { return _apiClient; }
        }

        private const string _api = "http://api.marya.app/api/";

        [Theory]
        [InlineData("niklasedelstam@protonmail.com", "Niklas89")]
        [InlineData("niklas_edelstam@hotmail.com", "Niklas89")]
        public async Task IsLogin_ReturnSuccessStatus_True(string email, string password)
        {
            // Arrange
            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(_api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string apiRoute = "auth/login";
            string requestUri = _api + apiRoute;
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
            });

            // Act
            HttpResponseMessage response = await _apiClient.PostAsync(apiRoute, data);
            //var result = await response.Content.ReadAsStringAsync();
            // _output.WriteLine($"Result for {requestUri}:\n{result}");

            // Assert
            Assert.True(response.IsSuccessStatusCode);

        }

        [Theory]
        [InlineData("test@qdzemail.com", "toto")]
        [InlineData("test213qsdq", "qsd")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public async Task IsLogin_ReturnSuccessStatus_False(string email, string password)
        {
            // Arrange
            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(_api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string apiRoute = "auth/login";
            string requestUri = _api + apiRoute;
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
            });

            // Act
            HttpResponseMessage response = await _apiClient.PostAsync(apiRoute, data);

            // Assert
            Assert.False(response.IsSuccessStatusCode);

        }
    }
}