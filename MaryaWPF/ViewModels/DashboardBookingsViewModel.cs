using Caliburn.Micro;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.ViewModels
{
    public class DashboardBookingsViewModel : Screen
    {
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

        private string _title;

        public string Title
        {
            get { return _title; }
            set 
            { 
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }


        public void UpdateDashboardBookings(BindingList<BookingDisplayModel>  bookings, string title)
        {
            Bookings = bookings;
            Title = title;
        }
    }
}
