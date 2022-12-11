using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Models
{
    public class ClientDisplayModel : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set 
            { 
                _phone = value;
                CallPropertyChanged(nameof(Phone));
            }
        }

        private string _address;

        public string Address
        {
            get { return _address; }
            set 
            { 
                _address = value; 
                CallPropertyChanged(nameof(Address));
            }
        }

        private string _postalCode;

        public string PostalCode
        {
            get { return _postalCode; }
            set 
            { 
                _postalCode = value; 
                CallPropertyChanged(nameof(PostalCode));
            }
        }

        private string _city;

        public string City
        {
            get { return _city; }
            set 
            { 
                _city = value; 
                CallPropertyChanged(nameof(City));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
