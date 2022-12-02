using MaryaWPF.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public interface IServiceEndpoint
    {
        Task<List<CategoryModel>> GetAll();
    }
}