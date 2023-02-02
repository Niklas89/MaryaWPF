using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public interface IProfileEndpoint
    {
        Task<bool> UpdateProfile(string email, string password);
        Task<bool> RegisterUser(string firstname, string lastname, string email, string password);
    }
}