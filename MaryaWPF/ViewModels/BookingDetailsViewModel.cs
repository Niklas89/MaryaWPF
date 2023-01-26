using Caliburn.Micro;
using LiveCharts.Wpf;
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

        public bool IsEditVisible
        {
            get
            {
                bool output = false;

                if (BookingCanBeEdited == true)
                {
                    output = true;
                }
                return output;
            }
        }

        private bool _bookingCanBeEdited;

        public bool BookingCanBeEdited
        {
            get { return _bookingCanBeEdited; }
            set
            {
                _bookingCanBeEdited = value;
                NotifyOfPropertyChange(() => IsEditVisible);
                NotifyOfPropertyChange(() => BookingCanBeEdited);
            }
        }

        public BookingDetailsViewModel(IBookingEndpoint bookingEndpoint)
        {
            _bookingEndpoint = bookingEndpoint;
        }


        public void UpdateBookingDetails(BookingDisplayModel selectedBooking)
        {

            SelectedBooking = selectedBooking;
            List<BookingDisplayModel> bookingList = new List<BookingDisplayModel>
            {
                selectedBooking
            };
            Bookings = new BindingList<BookingDisplayModel>(bookingList);

            BookingCanBeEdited = false;

            CheckIfBookingCanBeEdited();

        }

        // Check if the booking can be edited
        private void CheckIfBookingCanBeEdited()
        {
            if (!SelectedBooking.Accepted  && !SelectedBooking.IsCancelled) 
                BookingCanBeEdited = true;
        }

        public async void CancelBooking()
        {
            await _bookingEndpoint.RemoveBooking(SelectedBooking.Id);
        }

        public async void Edit()
        {

        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
