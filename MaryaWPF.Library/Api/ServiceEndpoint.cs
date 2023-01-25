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
    public class ServiceEndpoint : IServiceEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public ServiceEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<CategoryModel>> GetAll()
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

        public async Task<List<TypeModel>> GetAllTypes()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("admin/types"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<TypeModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<List<ServiceModel>> GetAllServicesByCategory(int id)
        {
            string uri = "admin/category/services/" + id;
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsAsync<List<ServiceModel>>();
                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task UpdateService(ServiceModel service)
        {
            string uri = "admin/category/service/" + service.Id;
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PutAsJsonAsync<ServiceModel>(uri, service))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task AddCategory(CategoryModel category)
        {
            string uri = "admin/category/";
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync<CategoryModel>(uri, category))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task AddService(ServiceModel service)
        {
            string uri = "admin/category/service/";
            using (HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync<ServiceModel>(uri, service))
            {
                if (response.IsSuccessStatusCode == false)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
