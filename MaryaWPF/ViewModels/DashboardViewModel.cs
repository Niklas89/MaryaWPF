using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaryaWPF.ViewModels
{
    public class DashboardViewModel : Screen
    {
        IBookingEndpoint _bookingEndpoint;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

        public DashboardViewModel(IBookingEndpoint bookingEndpoint, IMapper mapper, StatusInfoViewModel status, IWindowManager window)
        {
            _bookingEndpoint= bookingEndpoint;
            _mapper= mapper;
            _status = status;
            _window = window;
        }

        // When the page is loaded then we'll call OnViewLoaded
        // async void and not async Task because it's an event
        protected override async void OnViewLoaded(object view) 
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadBookings();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "Erreur Système";

                // Get a new ViewModel each time we call ShowDialogAsync(),
                // have multiple copies of StatusInfoViewModel inside the same class
                var info = IoC.Get<StatusInfoViewModel>();

                if(ex.Message == "Forbidden")
                {
                    _status.UpdateMessage("Accès refusé", "Vous n'avez pas l'autorisation de voir les réservations sur l'application bureautique.");
                    await _window.ShowDialogAsync(_status, null, settings);
                } else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                

                /* second message to show:
                _status.UpdateMessage("Accès refusé", "Vous n'avez pas l'autorisation de vous connecter sur l'application bureautique.");
                await _window.ShowDialogAsync(_status, null, settings);
                */

                await TryCloseAsync();
            }
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
