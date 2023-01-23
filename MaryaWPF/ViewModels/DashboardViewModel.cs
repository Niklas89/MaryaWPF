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
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private BookingDetailsViewModel _bookingDetails;

        private string _totalRevenue;

        public string TotalRevenue
        {
            get { return _totalRevenue; }
            set
            {
                _totalRevenue = value + " €";
                NotifyOfPropertyChange(() => TotalRevenue);
            }
        }

        private string _totalPriceAllBookings;

        public string TotalPriceAllBookings
        {
            get { return _totalPriceAllBookings; }
            set
            {
                _totalPriceAllBookings = "Réservations totales: " + value;
                NotifyOfPropertyChange(() => TotalPriceAllBookings);
            }
        }

        private string _totalPriceAcceptedBookings;

        public string TotalPriceAcceptedBookings
        {
            get { return _totalPriceAcceptedBookings; }
            set
            {
                _totalPriceAcceptedBookings = "Réservations accepteés: " + value;
                NotifyOfPropertyChange(() => TotalPriceAcceptedBookings);
            }
        }

        private string _totalPriceNotAcceptedBookings;

        public string TotalPriceNotAcceptedBookings
        {
            get { return _totalPriceNotAcceptedBookings; }
            set
            {
                _totalPriceNotAcceptedBookings = "Réservations en attente: " + value;
                NotifyOfPropertyChange(() => TotalPriceNotAcceptedBookings);
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
                ViewBookingDetails();
            }
        }


        public DashboardViewModel(IBookingEndpoint bookingEndpoint, IMapper mapper, StatusInfoViewModel status,
            IWindowManager window, BookingDetailsViewModel bookingDetails)
        {
            _bookingEndpoint = bookingEndpoint;
            _mapper = mapper;
            _status = status;
            _window = window;
            _bookingDetails = bookingDetails;
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


        // BOOKINGS CHART =======================================================================================================

        private async Task LoadBookings()
        {

            var bookingList = await _bookingEndpoint.GetAll();
            // AutoMapper nuget : link source model in MaryaWPF.Library to destination model in MaryaWPF
            List<BookingDisplayModel> bookings = _mapper.Map<List<BookingDisplayModel>>(bookingList);

            // List in order to view not accepted bookings in a datagrid that are superior to today
            List<BookingDisplayModel> bookingsListNotAccepted = bookings.Where(booking => booking.Accepted == false && booking.AppointmentDate.Month == DateTime.Now.Month)
                .OrderBy(b => b.AppointmentDate).ToList();
            Bookings = new BindingList<BookingDisplayModel>(bookingsListNotAccepted);

            // List for the chart
            List<BookingDisplayModel> bookingsAccepted = bookings.Where(x => x.Accepted == true && x.AppointmentDate.Month == DateTime.Now.Month).ToList();

            // List for the chart
            List<BookingDisplayModel> bookingsNotAccepted = bookings.Where(x => x.Accepted == false && x.AppointmentDate.Month == DateTime.Now.Month).ToList();

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
            settings.Height = 600;
            settings.Width = 600;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Détails de la réservation";

            _bookingDetails.UpdateBookingDetails(SelectedBooking);
            await _window.ShowDialogAsync(_bookingDetails, null, settings);

        }

    }
}
