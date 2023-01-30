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
    public class PartnerEndpoint : IPartnerEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public PartnerEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<UserPartnerModel>> GetAll()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("admin/partners"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<UserPartnerModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<UserPartnerModel>> GetAllRecruted()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("admin/partner/recruted"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<UserPartnerModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task UpdatePartner(UserPartnerModel partner)
        {

            // UserPartnerModel data = new UserPartnerModel { FirstName = partner.FirstName, LastName = partner.LastName };
            string uri = "admin/partner/" + partner.Id;
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync<UserPartnerModel>(uri, partner))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task AddPartner(UserPartnerModel partner)
        {

            // UserPartnerModel data = new UserPartnerModel { FirstName = partner.FirstName, LastName = partner.LastName };
            string uri = "admin/partner/";
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync<UserPartnerModel>(uri, partner))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.Content.ReadAsStringAsync().Result.Replace("\"", ""));
                }
            }
        }

        public async Task DeletePartner(UserPartnerModel partner)
        {
            // UserClientModel data = new UserClientModel { FirstName = client.FirstName, LastName = client.LastName };
            string uri = "admin/inactivate/" + partner.Id;
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync<UserPartnerModel>(uri, partner))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<CategoryModel>> GetAllCategories()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("admin/categories"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<CategoryModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
