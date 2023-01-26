using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Models
{
    public class PartnerDisplayModel : INotifyPropertyChanged
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

        private DateTime? _birthdate;

        public DateTime? Birthdate
        {
            get { return _birthdate; }
            set 
            { 
                _birthdate = value; 
                CallPropertyChanged(nameof(Birthdate));
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

        private string _siret;

        public string SIRET
        {
            get { return _siret; }
            set 
            {
                _siret = value; 
                CallPropertyChanged(nameof(SIRET));
            }
        }

        private string _iban;

        public string IBAN
        {
            get { return _iban; }
            set 
            {
                _iban = value; 
                CallPropertyChanged(nameof(IBAN));
            }
        }

        private int _idCategory;

        public int IdCategory
        {
            get { return _idCategory; }
            set 
            { 
                _idCategory = value;
                CallPropertyChanged(nameof(IdCategory));
            }
        }

        private string _categoryName;

        public string CategoryName
        {
            get { return _categoryName; }
            set 
            { 
                _categoryName = value; 
                CallPropertyChanged(nameof(CategoryName));
            }
        }

        public string Img { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
