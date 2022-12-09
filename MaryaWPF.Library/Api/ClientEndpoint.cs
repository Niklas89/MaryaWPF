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
    public class ClientEndpoint : IClientEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public ClientEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<UserClientModel>> GetAll()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("admin/clients"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<UserClientModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task UpdateClient(UserClientModel client)
        {

            // UserClientModel data = new UserClientModel { FirstName = client.FirstName, LastName = client.LastName };
            string uri = "admin/client/" + client.Id;
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync<UserClientModel>(uri, client))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
