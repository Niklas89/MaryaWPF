using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using System.Linq;
using System.Globalization;

namespace MaryaWPF.ViewModels
{
    public class DashboardViewModel : Screen
    {
        IBookingEndpoint _bookingEndpoint;
        IServiceEndpoint _serviceEndpoint;
        IClientEndpoint _clientEndpoint;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private BookingDetailsViewModel _bookingDetails;
        private DashboardBookingsViewModel _dashboardBookings;
        private DateTime _dateMonth;
        private string _totalRevenue;
        private string _totalPriceAllBookings;
        private string _totalPriceNotAcceptedBookings;
        private BindingList<BookingDisplayModel> _bookings;
        private string _totalPriceAcceptedBookings;
        private BookingDisplayModel _selectedBooking;
        private BindingList<BookingDisplayModel> _allBookingsThisMonth;
        private BindingList<BookingDisplayModel> _acceptedBookingsThisMonth;

        public DashboardViewModel(IBookingEndpoint bookingEndpoint, IMapper mapper, StatusInfoViewModel status,
            IWindowManager window, BookingDetailsViewModel bookingDetails, IServiceEndpoint serviceEndpoint, 
            IClientEndpoint clientEndpoint, DashboardBookingsViewModel dashboardBookings)
        {
            _bookingEndpoint = bookingEndpoint;
            _serviceEndpoint = serviceEndpoint;
            _clientEndpoint = clientEndpoint;
            _mapper = mapper;
            _status = status;
            _window = window;
            DateMonth = DateTime.Now;
            _bookingDetails = bookingDetails;
            _dashboardBookings = dashboardBookings;
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



        public string TotalRevenue
        {
            get { return _totalRevenue; }
            set
            {
                _totalRevenue = value + " €";
                NotifyOfPropertyChange(() => TotalRevenue);
            }
        }



        public string TotalPriceAllBookings
        {
            get { return _totalPriceAllBookings; }
            set
            {
                _totalPriceAllBookings = "Réservations totales: " + value;
                NotifyOfPropertyChange(() => TotalPriceAllBookings);
            }
        }



        public string TotalPriceAcceptedBookings
        {
            get { return _totalPriceAcceptedBookings; }
            set
            {
                _totalPriceAcceptedBookings = "Réservations accepteés: " + value;
                NotifyOfPropertyChange(() => TotalPriceAcceptedBookings);
            }
        }



        public string TotalPriceNotAcceptedBookings
        {
            get { return _totalPriceNotAcceptedBookings; }
            set
            {
                _totalPriceNotAcceptedBookings = "Réservations en attente: " + value;
                NotifyOfPropertyChange(() => TotalPriceNotAcceptedBookings);
            }
        }



        public BindingList<BookingDisplayModel> Bookings
        {
            get { return _bookings; }
            set
            {
                _bookings = value;
                NotifyOfPropertyChange(() => Bookings);
            }
        }



        public BookingDisplayModel SelectedBooking
        {
            get { return _selectedBooking; }
            set
            {
                _selectedBooking = value;
                //SelectedBookingId = value.Id;
                NotifyOfPropertyChange(() => SelectedBooking);
                ViewBookingDetails();
            }
        }


        public BindingList<BookingDisplayModel> AllBookingsThisMonth
        {
            get { return _allBookingsThisMonth; }
            set
            {
                _allBookingsThisMonth = value;
                NotifyOfPropertyChange(() => AllBookingsThisMonth);
            }
        }


        public BindingList<BookingDisplayModel> AcceptedBookingsThisMonth
        {
            get { return _acceptedBookingsThisMonth; }
            set
            {
                _acceptedBookingsThisMonth = value;
                NotifyOfPropertyChange(() => AcceptedBookingsThisMonth);
            }
        }


        // When the page is loaded then we'll call OnViewLoaded
        // async void and not async Task because it's an event
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadBookings();
                await LoadServicesForBookings();
                await LoadClientsForBookings();
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

        private async Task LoadServicesForBookings()
        {
            var serviceList = await _serviceEndpoint.GetAllServices();
            var services = _mapper.Map<List<ServiceDisplayModel>>(serviceList);

            foreach(var booking in Bookings)
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


        // BOOKINGS GRID =======================================================================================================

        private async Task LoadBookings()
        {

            var bookingList = await _bookingEndpoint.GetAll();
            // AutoMapper nuget : link source model in MaryaWPF.Library to destination model in MaryaWPF
            List<BookingDisplayModel> bookings = _mapper.Map<List<BookingDisplayModel>>(bookingList);

            // List in order to view not accepted and not cancelled bookings in a datagrid of this month
            List<BookingDisplayModel> bookingsListNotAccepted = bookings.Where(booking => booking.Accepted == false && !booking.IsCancelled && booking.AppointmentDate.Month == DateTime.Now.Month)
            .OrderByDescending(b => b.AppointmentDate).ToList();

            // List in order to view not accepted and not cancelled bookings in a datagrid - past and future
            // List<BookingDisplayModel> bookingsListNotAccepted = bookings.Where(booking => booking.Accepted == false && !booking.IsCancelled)
            //    .OrderByDescending(b => b.AppointmentDate).ToList();

            Bookings = new BindingList<BookingDisplayModel>(bookingsListNotAccepted);

            // Load the total of all the bookings this month : when you click on the card it shows in the pop-up
            List<BookingDisplayModel> allBookingsThisMonthList = bookings.Where(booking => booking.AppointmentDate.Month == DateTime.Now.Month).OrderByDescending(b => b.AppointmentDate).ToList();
            AllBookingsThisMonth = new BindingList<BookingDisplayModel>(allBookingsThisMonthList);

            // List for the chart
            List<BookingDisplayModel> bookingsAccepted = bookings.Where(x => x.Accepted == true && !x.IsCancelled && x.AppointmentDate.Month == DateTime.Now.Month).ToList();
            
            // List the accepted bookings of this month : when you click on the card it shows in the pop-up
            AcceptedBookingsThisMonth = new BindingList<BookingDisplayModel>(bookingsAccepted.OrderByDescending(b => b.AppointmentDate).ToList());

            // List for the chart
            List<BookingDisplayModel> bookingsNotAccepted = bookings.Where(x => x.Accepted == false && !x.IsCancelled && x.AppointmentDate.Month == DateTime.Now.Month).ToList();

            // Total Sum of bookings in Textblock above the chart
            List<BookingDisplayModel> bookingsThisMonth = bookings.Where(x => x.AppointmentDate.Month == DateTime.Now.Month).ToList();
            TotalPriceAllBookings = bookingsThisMonth.Count().ToString();
            TotalPriceAcceptedBookings = bookingsAccepted.Count().ToString();
            TotalPriceNotAcceptedBookings = bookingsNotAccepted.Count().ToString();
            TotalRevenue = bookingsThisMonth.Sum(x => x.TotalPrice).ToString();
        }

        public async void ViewBookingDetails()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 450;
            settings.Width = 700;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Détails de la réservation";

            _bookingDetails.UpdateBookingDetails(SelectedBooking);
            await _window.ShowDialogAsync(_bookingDetails, null, settings);

        }

        public async void ViewAllBookingsThisMonth()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 1000;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Toutes les réservations en " + DateMonth.ToString("MMMM yyyy");

            _dashboardBookings.UpdateDashboardBookings(AllBookingsThisMonth, settings.Title);
            await _window.ShowDialogAsync(_dashboardBookings, null, settings);
        }


        public async void ViewNotAcceptedBookingsThisMonth()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 1000;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Réservations en attente de " + DateMonth.ToString("MMMM yyyy");

            _dashboardBookings.UpdateDashboardBookings(Bookings, settings.Title);
            await _window.ShowDialogAsync(_dashboardBookings, null, settings);
        }


        public async void ViewAcceptedBookingsThisMonth()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 1000;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Réservations acceptés de " + DateMonth.ToString("MMMM yyyy");

            _dashboardBookings.UpdateDashboardBookings(AcceptedBookingsThisMonth, settings.Title);
            await _window.ShowDialogAsync(_dashboardBookings, null, settings);
        }

    }
}
