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
using System.ComponentModel;
using System.Threading;
using MaryaWPF.Commands;
using System.Collections;
using MaterialDesignColors.Recommended;

namespace MaryaWPF.ViewModels
{
    public class StatsViewModel: Screen
    {
        IBookingEndpoint _bookingEndpoint;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;

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


        // BOOKINGS CHART =======================================
        private Func<double, string> _bookingsFormatter;

        public event EventHandler<ActivationProcessedEventArgs> ActivationProcessed;

        public Func<double, string> BookingsFormatter
        {
            get { return _bookingsFormatter; }
            set
            {
                _bookingsFormatter = value => value.ToString("N");
            }
        }

        //public string[] BarLabels { get; set; } = new[] { "janvier", "février", "mars", "avril" };
        public string[] BookingsBarLabels { get; set; }

        //public SeriesCollection SeriesCollection { get; set; }

        public SeriesCollection BookingsSeriesCollection { get; set; } = new SeriesCollection
        {
            new LineSeries
            {
                Title="0",
                Values = new ChartValues<double> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            }
        };

        public SeriesCollection PieSeriesCollection { get; set; } = new SeriesCollection
        {
            new PieSeries
            {
                Title="0",
                Values = new ChartValues<int> { 0 }
            }
        };


        public StatsViewModel(IBookingEndpoint bookingEndpoint, IMapper mapper, StatusInfoViewModel status,
            IWindowManager window)
        {
            _bookingEndpoint = bookingEndpoint;
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
            string bookingsAcceptedTitle = "Réservations acceptés";

            // List for the chart
            List<BookingDisplayModel> bookingsNotAccepted = bookings.Where(x => x.Accepted == false && x.AppointmentDate.Month == DateTime.Now.Month).ToList();
            string bookingsNotAcceptedTitle = "Réservations en attente";

            int daysOfCurrentMonth = System.DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

            BookingsBarLabels = new string[daysOfCurrentMonth];

            ChartValues<double> chartValuesBookingsAccepted = new ChartValues<double>();
            ChartValues<double> chartValuesBookingsNotAccepted = new ChartValues<double>();

            for (int i = 1; i <= daysOfCurrentMonth; i++)
            {
                chartValuesBookingsAccepted.Add(0);
                chartValuesBookingsNotAccepted.Add(0);
            }

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

            // PieSeries Add
            PieSeriesCollection.Add(new PieSeries
            {
                Title = "Acceptés",
                Values = new ChartValues<int> { bookingsAccepted.Count }
            });

            PieSeriesCollection.Add(new PieSeries
            {
                Title = "En Attente",
                Values = new ChartValues<int> { bookingsNotAccepted.Count }
            });

            // Remove the default PieSeries object
            foreach (var serie in PieSeriesCollection)
            {
                if (serie.Title.Equals("0"))
                {
                    PieSeriesCollection.Remove(serie);
                    break;
                }
            }
        }

        private void LoadBookingsChart(List<BookingDisplayModel> bookings, string title, ChartValues<double> chartValues)
        {
            // Insert in Dictionnary (key: days | value: sum of totalprice per day) the totalprice sum of each booking per day
            // The sum of the TotalPrices
            //var bookingsDic = bookings.GroupBy(x => x.AppointmentDate.Day).ToDictionary(g => g.Key, g => g.Sum(x => x.TotalPrice));
            // The sum of the number of bookings
            var bookingsDic = bookings.GroupBy(x => x.AppointmentDate.Day).ToDictionary(g => g.Key, g => g.Count());


            // Replace the initialized 0 values from ChartValuesBookings and replace them with the values of my Dictionnary
            int index = 1;
            foreach (double value in chartValues)
            {
                foreach (var item in bookingsDic)
                {
                    if (item.Key.Equals(index))
                    {
                        chartValues.Remove(value);
                        chartValues.Insert(index, item.Value);
                    }
                }
                index++;
            }

            int daysOfCurrentMonth = System.DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var test = BookingsBarLabels;
            for (int i = 1; i <= daysOfCurrentMonth; i++)
            {
                BookingsBarLabels[i - 1] = i.ToString();
            }

            BookingsSeriesCollection.Add(new LineSeries
            {
                Title = title,
                Values = chartValues
            });
        }

        public async void ViewStatsYear()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 1000;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Statistiques de l'année";

            await _window.ShowWindowAsync(IoC.Get<StatsYearViewModel>(), null, settings);
        } 
    }
}
