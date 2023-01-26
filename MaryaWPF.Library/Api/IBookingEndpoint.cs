using MaryaWPF.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Api
{
    public interface IBookingEndpoint
    {
        Task<List<BookingModel>> GetAll();
        Task RemoveBooking(int bookingId);
        Task<List<BookingModel>> GetAllBookingsByService(int id);
    }
}