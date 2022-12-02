using System.Net.Http;
using System.Threading.Tasks;
using MaryaWPF.Library.Models;

namespace MaryaWPF.Library.Api
{
    public interface IAPIHelper
    {
        HttpClient ApiClient { get; }
        Task<AuthenticatedUser> Authenticate(string email, string password);
        void LogOffUser();
        Task GetLoggedInUserInfo(string token);
    }
}