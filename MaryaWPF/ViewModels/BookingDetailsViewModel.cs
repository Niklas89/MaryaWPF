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
    public class BookingDetailsViewModel : Screen
    {

        public string Header { get; private set; }
        public string Message { get; private set; }

        //public BookingDisplayModel SelectedBooking { get; private set; }
        //private readonly BookingDisplayModel _selectedBooking;

        //private BindingList<BookingDisplayModel> _bookings;

        //public BindingList<BookingDisplayModel> Bookings
        //{
        //    get { return _bookings; }
        //    set
        //    {
        //        _bookings = value;
        //        NotifyOfPropertyChange(() => Bookings);
        //    }
        //}

        //public BookingDetailsViewModel(BookingDisplayModel selectedBooking)
        //{
        //    _selectedBooking = selectedBooking;
        //}

        //protected override async void OnViewLoaded(object view)
        //{
        //    base.OnViewLoaded(view);

        //}

        public void UpdateMessage(string header, string message)
        //public void UpdateMessage(BookingDisplayModel selectedBooking)
        {
            Header = header;
            Message = message;

            NotifyOfPropertyChange(() => Header);
            NotifyOfPropertyChange(() => Message);

            //_selectedBooking = selectedBooking;
            //NotifyOfPropertyChange(() => SelectedBooking);
            //List<BookingDisplayModel> bookingList = new List<BookingDisplayModel>
            //{
            //    selectedBooking
            //};
            //Bookings = new BindingList<BookingDisplayModel>(bookingList);

        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
