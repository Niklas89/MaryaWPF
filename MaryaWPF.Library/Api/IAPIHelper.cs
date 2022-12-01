using System.Threading.Tasks;
using MaryaWPF.Library.Models;

namespace MaryaWPF.Library.Api
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string email, string password);
        Task GetLoggedInUserInfo(string token);
    }
}