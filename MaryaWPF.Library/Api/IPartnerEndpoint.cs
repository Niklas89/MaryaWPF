using MaryaWPF.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public interface IPartnerEndpoint
    {
        Task<List<UserPartnerModel>> GetAll();
        Task AddPartner(UserPartnerModel partner);
        Task UpdatePartner(UserPartnerModel partner);
        Task DeletePartner(UserPartnerModel partner);
        Task<List<CategoryModel>> GetAllCategories();
        Task<List<UserPartnerModel>> GetAllRecruted();
    }
}