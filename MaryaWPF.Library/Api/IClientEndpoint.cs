using MaryaWPF.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public interface IClientEndpoint
    {
        Task<List<UserClientModel>> GetAll();
    }
}