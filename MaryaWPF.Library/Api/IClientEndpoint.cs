using MaryaWPF.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public interface IClientEndpoint
    {
        Task<List<UserClientModel>> GetAll();

        Task AddClient(UserClientModel client);

        Task UpdateClient(UserClientModel client);

        Task DeleteClient(UserClientModel client);
    }
}