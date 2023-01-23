using AutoMapper;
using Caliburn.Micro;
using LiveCharts.Wpf;
using LiveCharts;
using MaryaWPF.Library.Api;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaryaWPF.ViewModels
{
    public class StatsYearViewModel : Screen
    {
        IBookingEndpoint _bookingEndpoint;
        IClientEndpoint _clientEndpoint;
        IPartnerEndpoint _partnerEndpoint;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;


        // BOOKINGS CHART =======================================
        private Func<double, string> _bookingsFormatter;

        public Func<double, string> BookingsFormatter
        {
            get { return _bookingsFormatter; }
            set
            {
                _bookingsFormatter = value => value.ToString("N");
            }
        }

        //public string[] BarLabels { get; set; } = new[] { "janvier", "février", "mars", "avril" };
        public string[] BookingsBarLabels { get; set; } = new string[12];

        //public SeriesCollection SeriesCollection { get; set; }

        public SeriesCollection BookingsSeriesCollection { get; set; } = new SeriesCollection
        {
            new LineSeries
            {
                Title="0",
                Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }
        };


        // CLIENTS AND PARTNERS CHART =======================================
        private Func<double, string> _clientsPartnersFormatter;

        public Func<double, string> ClientsPartnersFormatter
        {
            get { return _clientsPartnersFormatter; }
            set
            {
                _clientsPartnersFormatter = value => value.ToString("N");
            }
        }

        public string[] ClientsPartnersBarLabels { get; set; } = new string[12];


        public SeriesCollection ClientsPartnersSeriesCollection { get; set; } = new SeriesCollection
        {
            new LineSeries
            {
                Title="0",
                Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }
        };

        public StatsYearViewModel(IBookingEndpoint bookingEndpoint, IMapper mapper, StatusInfoViewModel status,
            IWindowManager window, IClientEndpoint clientEndpoint, IPartnerEndpoint partnerEndpoint)
        {
            _bookingEndpoint = bookingEndpoint;
            _clientEndpoint = clientEndpoint;
            _partnerEndpoint = partnerEndpoint;
            _mapper = mapper;
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
                await LoadClientsPartners();
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

            List<BookingDisplayModel> bookingsAccepted = bookings.Where(x => x.Accepted == true).ToList();
            string bookingsAcceptedTitle = "Réservations acceptés";
            List<BookingDisplayModel> bookingsNotAccepted = bookings.Where(x => x.Accepted == false).ToList();
            string bookingsNotAcceptedTitle = "Réservations en attente";

            ChartValues<double> chartValuesBookingsAccepted = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            ChartValues<double> chartValuesBookingsNotAccepted = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            // Create LineSeries for SeriesCollection for chart values of accepted bookings
            LoadBookingsChart(bookingsAccepted, bookingsAcceptedTitle, chartValuesBookingsAccepted);

            // Create LineSeries for SeriesCollection for chart values of NOT accepted bookings
            LoadBookingsChart(bookingsNotAccepted, bookingsNotAcceptedTitle, chartValuesBookingsNotAccepted);

            // Remove the default LineSeries object
            foreach (var serie in BookingsSeriesCollection)
            {
                if (serie.Title.Equals("0"))
                {
                    BookingsSeriesCollection.Remove(serie);
                    break;
                }
            }
        }

        private void LoadBookingsChart(List<BookingDisplayModel> bookings, string title, ChartValues<double> chartValues)
        {
            // Insert in Dictionnary (key: month, year | value: number of bookings) the number of bookings per month for the 12 previous months
            var bookingsDic = bookings.GroupBy(x => new { Month = x.AppointmentDate.Month, Year = x.AppointmentDate.Year })
                .ToDictionary(g => g.Key, g => g.Count());

            // Remove the current month of the current year, because it shouldn't be visible on the chart
            if (bookingsDic.ContainsKey(new { DateTime.Now.Month, DateTime.Now.Year }))
            {
                bookingsDic.Remove(new { DateTime.Now.Month, DateTime.Now.Year });
            }

            // Replace the initialized 0 values from ChartValuesBookings and replace them with the values of my Dictionnary
            int index = 1;
            foreach (double value in chartValues)
            {
                foreach (var item in bookingsDic)
                {
                    if (item.Key.Month.Equals(index))
                    {
                        chartValues.Remove(value);
                        chartValues.Add(item.Value);
                    }
                }
                index++;
            }

            // Loop through each month of datetime.now -1 year (12 previous months)
            // Add insert each month in a new list of months, get name in string of each month
            List<int> monthsNumberOfPastYear = new List<int>();
            List<string> monthsNameOfPastYear = new List<string>();
            var lastYear = DateTime.Now.AddYears(-1);
            DateTime todayDate = DateTime.Now.AddMonths(-1);
            for (DateTime dt = lastYear; dt < todayDate; dt = dt.AddMonths(1))
            {
                monthsNumberOfPastYear.Add(dt.Month);
                monthsNameOfPastYear.Add(getFullName(dt.Month));
            }

            int monthsCount = monthsNumberOfPastYear.Count();
            for (int i = 0; i < monthsCount; i++)
            {
                BookingsBarLabels[i] = monthsNameOfPastYear.ElementAt(i);
            }

            BookingsSeriesCollection.Add(new LineSeries
            {
                Title = title,
                Values = chartValues
            });
        }

        private string getFullName(int month)
        {
            return CultureInfo.CurrentCulture.
                DateTimeFormat.GetMonthName
                (month);
        }


        // CLIENTS AND PARTNERS CHART =======================================================================================================

        private async Task LoadClientsPartners()
        {
            var clientList = await _clientEndpoint.GetAll();
            var clients = _mapper.Map<List<UserClientDisplayModel>>(clientList);

            var partnerList = await _partnerEndpoint.GetAll();
            var partners = _mapper.Map<List<UserPartnerDisplayModel>>(partnerList);


            string clientsTitle = "Clients";
            string partnersTitle = "Partenaires";

            ChartValues<double> chartValuesClients = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            ChartValues<double> chartValuesPartners = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            // Create LineSeries for SeriesCollection for chart values of clients
            LoadClientsChart(clients, clientsTitle, chartValuesClients);

            // Create LineSeries for SeriesCollection for chart values of partners
            LoadPartnersChart(partners, partnersTitle, chartValuesPartners);

            // Remove the default LineSeries object
            foreach (var serie in ClientsPartnersSeriesCollection)
            {
                if (serie.Title.Equals("0"))
                {
                    ClientsPartnersSeriesCollection.Remove(serie);
                    break;
                }
            }
        }

        private void LoadClientsChart(List<UserClientDisplayModel> clients, string title, ChartValues<double> chartValues)
        {
            // Insert in Dictionnary (key: month, year | value: number of bookings) the number of bookings per month for the 12 previous months
            var clientsDic = clients.GroupBy(x => new { Month = x.CreatedAt.Month, Year = x.CreatedAt.Year })
                .ToDictionary(g => g.Key, g => g.Count());

            // Remove the current month of the current year, because it shouldn't be visible on the chart
            if (clientsDic.ContainsKey(new { DateTime.Now.Month, DateTime.Now.Year }))
            {
                clientsDic.Remove(new { DateTime.Now.Month, DateTime.Now.Year });
            }

            // Replace the initialized 0 values from ChartValuesBookings and replace them with the values of my Dictionnary
            int index = 1;
            foreach (double value in chartValues)
            {
                foreach (var item in clientsDic)
                {
                    if (item.Key.Month.Equals(index))
                    {
                        chartValues.Remove(value);
                        chartValues.Add(item.Value);
                    }
                }
                index++;
            }

            // Loop through each month of datetime.now -1 year (12 previous months)
            // Add insert each month in a new list of months, get name in string of each month
            List<int> monthsNumberOfPastYear = new List<int>();
            List<string> monthsNameOfPastYear = new List<string>();
            var lastYear = DateTime.Now.AddYears(-1);
            DateTime todayDate = DateTime.Now.AddMonths(-1);
            for (DateTime dt = lastYear; dt < todayDate; dt = dt.AddMonths(1))
            {
                monthsNumberOfPastYear.Add(dt.Month);
                monthsNameOfPastYear.Add(getFullName(dt.Month));
            }

            int monthsCount = monthsNumberOfPastYear.Count();
            for (int i = 0; i < monthsCount; i++)
            {
                ClientsPartnersBarLabels[i] = monthsNameOfPastYear.ElementAt(i);
            }

            ClientsPartnersSeriesCollection.Add(new LineSeries
            {
                Title = title,
                Values = chartValues
            });
        }

        private void LoadPartnersChart(List<UserPartnerDisplayModel> partners, string title, ChartValues<double> chartValues)
        {
            // Insert in Dictionnary (key: month, year | value: number of bookings) the number of bookings per month for the 12 previous months
            var partnersDic = partners.GroupBy(x => new { Month = x.CreatedAt.Month, Year = x.CreatedAt.Year })
                .ToDictionary(g => g.Key, g => g.Count());

            // Remove the current month of the current year, because it shouldn't be visible on the chart
            if (partnersDic.ContainsKey(new { DateTime.Now.Month, DateTime.Now.Year }))
            {
                partnersDic.Remove(new { DateTime.Now.Month, DateTime.Now.Year });
            }

            // Replace the initialized 0 values from ChartValuesBookings and replace them with the values of my Dictionnary
            int index = 1;
            foreach (double value in chartValues)
            {
                foreach (var item in partnersDic)
                {
                    if (item.Key.Month.Equals(index))
                    {
                        chartValues.Remove(value);
                        chartValues.Add(item.Value);
                    }
                }
                index++;
            }

            // Loop through each month of datetime.now -1 year (12 previous months)
            // Add insert each month in a new list of months, get name in string of each month
            List<int> monthsNumberOfPastYear = new List<int>();
            List<string> monthsNameOfPastYear = new List<string>();
            var lastYear = DateTime.Now.AddYears(-1);
            DateTime todayDate = DateTime.Now.AddMonths(-1);
            for (DateTime dt = lastYear; dt < todayDate; dt = dt.AddMonths(1))
            {
                monthsNumberOfPastYear.Add(dt.Month);
                monthsNameOfPastYear.Add(getFullName(dt.Month));
            }

            int monthsCount = monthsNumberOfPastYear.Count();
            for (int i = 0; i < monthsCount; i++)
            {
                ClientsPartnersBarLabels[i] = monthsNameOfPastYear.ElementAt(i);
            }

            ClientsPartnersSeriesCollection.Add(new LineSeries
            {
                Title = title,
                Values = chartValues
            });
        }

    }
}
