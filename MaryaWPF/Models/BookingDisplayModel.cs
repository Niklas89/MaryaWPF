using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Models
{
    public class BookingDisplayModel : INotifyPropertyChanged
    {
        // Only indicate the properties that we need to display
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int? NbHours { get; set; }
        public string Description { get; set; }
        public bool Accepted { get; set; }
        public float TotalPrice { get; set; }
        public DateTime? CancelDate { get; set; }
        public bool IsCancelled { get; set; }
        public bool ServiceDone { get; set; }
        public string ServiceDoneYesNo
        {
            get { return !ServiceDone ? "Non" : "Oui"; }
        }
        public bool IsPaid { get; set; }
        public string IsPaidYesNo
        {
            get { return !IsPaid ? "Non" : "Oui"; }
        }

        private int myVar;
        // Handle the display update
        public int MyProperty
        {
            get { return myVar; }
            set { 
                myVar = value;
                CallPropertyChanged(nameof(MyProperty));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
