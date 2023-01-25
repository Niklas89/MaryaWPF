using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Models
{
    public class ServiceDisplayModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                CallPropertyChanged(nameof(Name));
            }
        }

        private float _price;

        public float Price
        {
            get { return _price; }
            set 
            { 
                _price = value;
                CallPropertyChanged(nameof(Price));
            }
        }

        public int IdCategory { get; set; }
        public string CategoryName { get; set; }

        private int _idType;

        public int IdType
        {
            get { return _idType; }
            set
            {
                _idType = value;
                CallPropertyChanged(nameof(IdType));
            }
        }

        private string _typeName;

        public string TypeName
        {
            get { return _typeName; }
            set
            {
                _typeName = value;
                CallPropertyChanged(nameof(TypeName));
            }
        }

        private string _priceId;

        public string PriceId
        {
            get { return _priceId; }
            set 
            { 
                _priceId = value;
                CallPropertyChanged(nameof(PriceId));
            }
        }


        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
