using MaryaWPF.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public interface IServiceEndpoint
    {
        Task<List<CategoryModel>> GetAll();
        Task<List<ServiceModel>> GetAllServicesByCategory(int id);
        Task<List<TypeModel>> GetAllTypes();
        Task<List<ServiceModel>> GetAllServices();
        Task UpdateService(ServiceModel service);
        Task AddCategory(CategoryModel category);
        Task AddService(ServiceModel service);
        Task DeleteService(int id);
    }
}