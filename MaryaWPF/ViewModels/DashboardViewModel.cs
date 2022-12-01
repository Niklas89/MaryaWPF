using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.ViewModels
{
    public class DashboardViewModel : Screen
    {
        private IBookingEndpoint _bookingEndpoint;

        public DashboardViewModel(IBookingEndpoint bookingEndpoint)
        {
            _bookingEndpoint= bookingEndpoint;
        }

        // When the page is loaded then we'll call OnViewLoaded
        // async void and not async Task because it's an event
        protected override async void OnViewLoaded(object view) 
        {
            base.OnViewLoaded(view);
            await LoadBookings();
        }

        private async Task LoadBookings()
        {
            var bookingList = await _bookingEndpoint.GetAll();
            Bookings = new BindingList<BookingModel>(bookingList);
        }

        private BindingList<BookingModel> _bookings;

        public BindingList<BookingModel> Bookings
        {
            get { return _bookings; }
            set { 
                _bookings = value;
                NotifyOfPropertyChange(() => Bookings);
            }
        }
    }
}
