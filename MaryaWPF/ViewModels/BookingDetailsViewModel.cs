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

        public bool IsCancelVisible
        {
            get
            {
                bool output = false;

                if (BookingCanBeCancelled == true)
                {
                    output = true;
                }
                return output;
            }
        }

        private bool _bookingCanBeCancelled;

        public bool BookingCanBeCancelled
        {
            get { return _bookingCanBeCancelled; }
            set
            {
                _bookingCanBeCancelled = value;
                NotifyOfPropertyChange(() => IsCancelVisible);
                NotifyOfPropertyChange(() => BookingCanBeCancelled);
            }
        }

        public BookingDetailsViewModel(IBookingEndpoint bookingEndpoint)
        {
            _bookingEndpoint = bookingEndpoint;
        }


        public void UpdateBookingDetails(BookingDisplayModel selectedBooking)
        {
            if(selectedBooking != null)
            {
                SelectedBooking = selectedBooking;
                List<BookingDisplayModel> bookingList = new List<BookingDisplayModel>
                {
                selectedBooking
                };
                Bookings = new BindingList<BookingDisplayModel>(bookingList);

                BookingCanBeCancelled = false;

                CheckIfBookingCanBeCancelled();
            } else
            {
                Close();
            }
        }

        // Check if the booking can be cancelled
        private async Task CheckIfBookingCanBeCancelled()
        {
            if (!SelectedBooking.Accepted && !SelectedBooking.IsCancelled)
                BookingCanBeCancelled = true;
        }


        public async void CancelBooking()
        {
            await _bookingEndpoint.RemoveBooking(SelectedBooking.Id);

            // Below lines are USEFUL for INotifyPropertyChange in BookingDisplayModel
            SelectedBooking.IsCancelled = true;
            SelectedBooking.CancelDate = DateTime.Now;

            Close();
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
