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
        public string AcceptedYesNo
        {
            get { return !Accepted ? "Non" : "Oui"; }
        }

        public float TotalPrice { get; set; }

        private DateTime? _cancelDate;

        public DateTime? CancelDate
        {
            get { return _cancelDate; }
            set 
            { 
                _cancelDate = value;
                CallPropertyChanged(nameof(CancelDate));
            }
        }


        private bool _isCancelled;
        public bool IsCancelled
        {
            get { return _isCancelled; }
            set 
            { 
                _isCancelled = value;
                CallPropertyChanged(nameof(IsCancelledYesNo));
                CallPropertyChanged(nameof(IsCancelled));
            }
        }

        public string IsCancelledYesNo
        {
            get { return !IsCancelled ? "Non" : "Oui"; }
        }

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

        public DateTime CreatedAt { get; set; }

        public int IdClient { get; set; }

        private string _clientFullName;
        public string ClientFullName
        {
            get { return _clientFullName; }
            set
            {
                _clientFullName = value;
                CallPropertyChanged(nameof(ClientFullName));
            }
        }

        public int? IdPartner { get; set; }

        private string _partnerFullName;
        public string PartnerFullName
        {
            get { return _partnerFullName; }
            set
            {
                _partnerFullName = value;
                CallPropertyChanged(nameof(PartnerFullName));
            }
        }

        public int IdService { get; set; }

        private string _serviceName;
        public string ServiceName
        {
            get { return _serviceName; }
            set {
                _serviceName = value;
                CallPropertyChanged(nameof(ServiceName));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
