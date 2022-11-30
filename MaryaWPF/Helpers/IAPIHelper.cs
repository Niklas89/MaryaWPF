using MaryaWPF.Models;
using System.Threading.Tasks;

namespace MaryaWPF.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string email, string password);
    }
}