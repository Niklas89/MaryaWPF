using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MaryaWPF.Library;
using MaryaWPF.Library.Models;

namespace MaryaWPF.Library.Api
{
    // the APIHelper is active in the whole application (see singleton in Bootstrapper) when it is instanciated
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient;
        private ILoggedInUserModel _loggedInUser;

        public APIHelper(ILoggedInUserModel loggedInUser)
        {
            InitializeClient();
            _loggedInUser = loggedInUser;
        }

        public HttpClient ApiClient
        {
            get { return _apiClient; }
        }

        private void InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"]; 

            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string email, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                // new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
            });

            using (HttpResponseMessage response = await _apiClient.PostAsync("auth/login", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    // installer AspNet.WebApi.Client nuget package et Newtonsoft.Json pour faire fonctionner le ReadAsAsync du JSON
                    // var result = await response.Content.ReadAsAsync<UserModel>();
                    var result = await response.Content.ReadAsStringAsync();

                    dynamic DynamicData = JsonConvert.DeserializeObject(result);
                    var user = new AuthenticatedUser()
                    {
                        AccessToken = DynamicData.accessToken, // accessToken needed for GetLoggedInUserInfo() - passed in from LogIn() in LoginViewModel.cs
                       //Email = DynamicData.user.email
                    };
                    return user;
                }
                else
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                }
            }
        }

        public void LogOffUser()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }

        // Add a new header to our request with the access token, with every call made
        // Make sure that we send our credentials with every request
        public async Task GetLoggedInUserInfo(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using (HttpResponseMessage response = await _apiClient.GetAsync("admin/profile"))
            {
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<LoggedInUserModel>();
                    _loggedInUser.Id = result.Id;
                    _loggedInUser.FirstName = result.FirstName;
                    _loggedInUser.LastName = result.LastName;
                    _loggedInUser.Email= result.Email;
                    _loggedInUser.CreatedAt = result.CreatedAt;
                    _loggedInUser.Token = token;
                    // We don't need to save the result anywhere, it's a singleton,
                    // so once it's updated here it's updated everywhere. See Bootstrapper.cs
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
