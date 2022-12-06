using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.ViewModels
{
    public class BookingDetailsViewModel : Screen
    {

        //public string Header { get; private set; }
        //public string Message { get; private set; }

        //public BookingDisplayModel SelectedBooking { get; private set; }
        IBookingEndpoint _bookingEndpoint;

        private BookingDisplayModel _selectedBooking;
        public BookingDisplayModel SelectedBooking
        {
            get { return _selectedBooking; }
            set
            {
                _selectedBooking = value;
                NotifyOfPropertyChange(() => SelectedBooking);
            }
        }
        

        private BindingList<BookingDisplayModel> _bookings;

        public BindingList<BookingDisplayModel> Bookings
        {
            get { return _bookings; }
            set
            {
                _bookings = value;
                NotifyOfPropertyChange(() => Bookings);
            }
        }

        public BookingDetailsViewModel(IBookingEndpoint bookingEndpoint)
        {
            _bookingEndpoint = bookingEndpoint;
        }


        //public void UpdateMessage(string header, string message)
        public void UpdateBookingDetails(BookingDisplayModel selectedBooking)
        {
            //Header = header;
            //Message = message;

            //NotifyOfPropertyChange(() => Header);
            //NotifyOfPropertyChange(() => Message);

            _selectedBooking = selectedBooking;
           // NotifyOfPropertyChange(() => SelectedBooking);
            List<BookingDisplayModel> bookingList = new List<BookingDisplayModel>
            {
                selectedBooking
            };
            Bookings = new BindingList<BookingDisplayModel>(bookingList);

        }

        public async Task CancelBooking()
        {
            await _bookingEndpoint.RemoveBooking(SelectedBooking.Id);
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
