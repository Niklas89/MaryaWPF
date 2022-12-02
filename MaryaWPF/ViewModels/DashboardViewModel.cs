using AutoMapper;
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
    public class DashboardViewModel : Screen
    {
        IBookingEndpoint _bookingEndpoint;
        IMapper _mapper;

        public DashboardViewModel(IBookingEndpoint bookingEndpoint, IMapper mapper)
        {
            _bookingEndpoint= bookingEndpoint;
            _mapper= mapper;
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
            // AutoMapper nuget : link source model in MaryaWPF.Library to destination model in MaryaWPF
            var bookings = _mapper.Map<List<BookingDisplayModel>>(bookingList);
            Bookings = new BindingList<BookingDisplayModel>(bookings);
        }

        private BindingList<BookingDisplayModel> _bookings;

        public BindingList<BookingDisplayModel> Bookings
        {
            get { return _bookings; }
            set { 
                _bookings = value;
                NotifyOfPropertyChange(() => Bookings);
            }
        }
    }
}
