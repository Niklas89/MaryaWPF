using MaryaWPF.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public interface IPartnerEndpoint
    {
        Task<List<UserPartnerModel>> GetAll();
        Task UpdatePartner(UserPartnerModel partner);
    }
}