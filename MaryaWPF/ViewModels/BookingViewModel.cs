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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MaryaWPF.Views;

namespace MaryaWPF.ViewModels
{
    public class BookingViewModel : Screen
    {
        IBookingEndpoint _bookingEndpoint;
        IServiceEndpoint _serviceEndpoint;
        IClientEndpoint _clientEndpoint;
        IPartnerEndpoint _partnerEndpoint;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private BookingDetailsViewModel _bookingDetails;
        private DateTime _dateMonth;
        private bool _isCurrentMonth;

        public BookingViewModel(IBookingEndpoint bookingEndpoint, IMapper mapper, StatusInfoViewModel status,
            IWindowManager window, BookingDetailsViewModel bookingDetails, IServiceEndpoint serviceEndpoint,
            IClientEndpoint clientEndpoint, IPartnerEndpoint partnerEndpoint)
        {
            _bookingEndpoint = bookingEndpoint;
            _partnerEndpoint = partnerEndpoint;
            _clientEndpoint = clientEndpoint;
            _serviceEndpoint = serviceEndpoint;
            _mapper = mapper;
            _status = status;
            _bookingDetails = bookingDetails;
            _window = window;
            DateMonth = DateTime.Now;
            IsCurrentMonth = false;
        }


        // Month displayed on top of the page
        public DateTime DateMonth
        {
            get { return _dateMonth; }
            set
            {
                _dateMonth = value;
                NotifyOfPropertyChange(() => DateMonth);
            }
        }


        public bool IsCurrentMonth
        {
            get { return _isCurrentMonth; }
            set
            {
                _isCurrentMonth = value;
                NotifyOfPropertyChange(() => IsCurrentMonth);
            }
        }

        // When the page is loaded then we'll call OnViewLoaded
        // async void and not async Task because it's an event
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            await LoadViewServices();
        }

        private async Task LoadViewServices()
        {
            try
            {
                await LoadBookings();
                await LoadServicesForBookings();
                await LoadClientsForBookings();
                await LoadPartnersForBookings();
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

                if (ex.Message == "Forbidden")
                {
                    _status.UpdateMessage("Accès refusé", "Vous n'avez pas l'autorisation de voir les réservations sur l'application bureautique.");
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                else
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
            var orderedBookings = bookings.Where(booking => booking.AppointmentDate.Month == DateMonth.Month).OrderByDescending(b => b.AppointmentDate).ToList();
            Bookings = new BindingList<BookingDisplayModel>(orderedBookings);
        }

        private async Task LoadServicesForBookings()
        {
            var serviceList = await _serviceEndpoint.GetAllServices();
            var services = _mapper.Map<List<ServiceDisplayModel>>(serviceList);

            foreach (var booking in Bookings)
            {
                foreach (var service in services)
                {
                    if (booking.IdService == service.Id)
                    {
                        booking.ServiceName = service.Name;
                    }
                }
            }
        }

        private async Task LoadClientsForBookings()
        {
            var clientList = await _clientEndpoint.GetAll();
            var clients = _mapper.Map<List<UserClientDisplayModel>>(clientList);

            foreach (var booking in Bookings)
            {
                foreach (var client in clients)
                {
                    if (booking.IdClient == client.Client.Id)
                    {
                        booking.ClientFullName = client.FirstName + " " + client.LastName;
                    }
                }
            }
        }

        private async Task LoadPartnersForBookings()
        {
            var partnerList = await _partnerEndpoint.GetAll();
            var partners = _mapper.Map<List<UserPartnerDisplayModel>>(partnerList);

            foreach (var booking in Bookings)
            {
                foreach (var partner in partners)
                {
                    if (booking.IdPartner == partner.Partner.Id)
                    {
                        booking.PartnerFullName = partner.FirstName + " " + partner.LastName;
                    }
                }
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

        private BookingDisplayModel _selectedBooking;

        public BookingDisplayModel SelectedBooking
        {
            get { return _selectedBooking; }
            set
            {
                _selectedBooking = value;
                //SelectedBookingId = value.Id;
                NotifyOfPropertyChange(() => SelectedBooking);
                if(SelectedBooking != null) ViewBookingDetails();
            }
        }

        //private int _selectedBookingId;

        //public int SelectedBookingId
        //{
        //    get { return _selectedBookingId; }
        //    set { 
        //        _selectedBookingId = value;
        //        NotifyOfPropertyChange(() => SelectedBookingId);
        //    }
        //}


        public async void ViewBookingDetails()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 500;
            settings.Width = 700;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Détails de la réservation";

            _bookingDetails.UpdateBookingDetails(SelectedBooking);
            await _window.ShowDialogAsync(_bookingDetails, null, settings);

        }

        public async void PreviousMonth()
        {
            DateMonth = DateMonth.AddMonths(-1);
            IsCurrentMonth = true;
            await LoadViewServices();
        }

        public async void NextMonth()
        {
            DateTime nextMonth = DateMonth.AddMonths(1);
            if (nextMonth.Month == DateTime.Now.Month)
            {
                IsCurrentMonth = false;
            }
            DateMonth = nextMonth;
            await LoadViewServices();

        }
    }
}
