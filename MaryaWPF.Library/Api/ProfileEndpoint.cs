using MaryaWPF.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public class ProfileEndpoint : IProfileEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public ProfileEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<bool> UpdateProfile(string email, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
            });

            string uri = "admin/profile/";
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsync(uri, data))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                }
            }

            return true;
        }

        public async Task<bool> RegisterUser(string firstName, string lastName, string email, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("firstName", firstName),
                new KeyValuePair<string, string>("lastName", lastName),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
            });

            string uri = "auth/admin/register";
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsync(uri, data))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                }
            }

            return true;
        }
    }
}
