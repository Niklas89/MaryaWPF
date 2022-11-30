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
using MaryaWPF.Models;

namespace MaryaWPF.Helpers
{
    // the APIHelper is active in the whole application (see singleton in Bootstrapper) when it is instanciated
    public class APIHelper : IAPIHelper 
    {
        private HttpClient apiClient { get; set; }

        public APIHelper()
        {
            InitializeClient();
        }

        private void InitializeClient()
        {
            string api = ConfigurationManager.AppSettings["api"];
            apiClient = new HttpClient();
            apiClient.BaseAddress = new Uri(api);
            apiClient.DefaultRequestHeaders.Accept.Clear();
            apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUser> Authenticate(string email, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                // new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
            });

            using (HttpResponseMessage response = await apiClient.PostAsync("auth/login", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    // installer AspNet.WebApi.Client nuget package et Newtonsoft.Json pour faire fonctionner le ReadAsAsync du JSON
                    // var result = await response.Content.ReadAsAsync<UserModel>();
                    var result = await response.Content.ReadAsStringAsync();

                    dynamic DynamicData = JsonConvert.DeserializeObject(result);
                    var user = new AuthenticatedUser()
                    {
                        AccessToken = DynamicData.accessToken,
                        Email = DynamicData.user.email
                    };
                    return user;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
