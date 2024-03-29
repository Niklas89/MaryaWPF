﻿using MaryaWPF.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Models
{
    public class UserClientDisplayModel : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                CallPropertyChanged(nameof(FirstName));
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set 
            { 
                _lastName = value;
                CallPropertyChanged(nameof(LastName));
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set 
            { 
                _email = value; 
                CallPropertyChanged(nameof(Email));
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                CallPropertyChanged(nameof(Password));
            }
        }

        private bool _isActive;
        public bool IsActive {
            get { return _isActive; }
            set
            {
                _isActive = value;
                CallPropertyChanged(nameof(IsActiveYesNo));
                CallPropertyChanged(nameof(IsActive));
            }
        }
        public string IsActiveYesNo
        {
            get { return !IsActive ? "Non" : "Oui"; }
        }
        public DateTime? DeactivatedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public ClientDisplayModel Client { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
