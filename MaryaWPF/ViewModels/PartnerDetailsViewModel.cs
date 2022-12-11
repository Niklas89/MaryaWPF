using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.ViewModels
{
    public class PartnerDetailsViewModel: Screen
    {
        IPartnerEndpoint _partnerEndpoint;
        IMapper _mapper;
        private UserPartnerDisplayModel _selectedPartner;
        public UserPartnerDisplayModel SelectedPartner
        {
            get { return _selectedPartner; }
            set
            {
                _selectedPartner = value;
                NotifyOfPropertyChange(() => SelectedPartner);
            }
        }


        private BindingList<UserPartnerDisplayModel> _partners;

        public BindingList<UserPartnerDisplayModel> Partners
        {
            get { return _partners; }
            set
            {
                _partners = value;
                NotifyOfPropertyChange(() => Partners);
            }
        }

        public PartnerDetailsViewModel(IPartnerEndpoint partnerEndpoint, IMapper mapper)
        {
            _partnerEndpoint = partnerEndpoint;
            _mapper = mapper;
        }
        private string _selectedFirstName;

        public string SelectedFirstName
        {
            get { return _selectedFirstName; }
            set
            {
                _selectedFirstName = value;
                NotifyOfPropertyChange(() => SelectedFirstName);
            }
        }

        private string _selectedLastName;

        public string SelectedLastName
        {
            get { return _selectedLastName; }
            set
            {
                _selectedLastName = value;
                NotifyOfPropertyChange(() => SelectedLastName);
            }
        }

        private string _selectedPhone;
        public string SelectedPhone
        {
            get { return _selectedPhone; }
            set
            {
                _selectedPhone = value;
                NotifyOfPropertyChange(() => SelectedPhone);
            }
        }
        private string _selectedEmail;
        public string SelectedEmail
        {
            get { return _selectedEmail; }
            set
            {
                _selectedEmail = value;
                NotifyOfPropertyChange(() => SelectedEmail);
            }
        }
        private string _selectedAddress;
        public string SelectedAddress
        {
            get { return _selectedAddress; }
            set
            {
                _selectedAddress = value;
                NotifyOfPropertyChange(() => SelectedAddress);
            }
        }
        private string _selectedCity;
        public string SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                NotifyOfPropertyChange(() => SelectedCity);
            }
        }
        private string _selectedPostalCode;
        public string SelectedPostalCode
        {
            get { return _selectedPostalCode; }
            set
            {
                _selectedPostalCode = value;
                NotifyOfPropertyChange(() => SelectedPostalCode);
            }
        }
        private DateTime? _selectedBirthdate;
        public DateTime? SelectedBirthdate
        {
            get { return _selectedBirthdate; }
            set { 
                _selectedBirthdate = value;
                NotifyOfPropertyChange(() => SelectedBirthdate); 
            }
        }


        public void UpdatePartnerDetails(UserPartnerDisplayModel selectedPartner)
        {
            _selectedPartner = selectedPartner;
            List<UserPartnerDisplayModel> partnerList = new List<UserPartnerDisplayModel>
            {
                selectedPartner
            };
            Partners = new BindingList<UserPartnerDisplayModel>(partnerList);
            SelectedFirstName = selectedPartner.FirstName;
            SelectedLastName = selectedPartner.LastName;
            SelectedPhone = selectedPartner.Partner.Phone;
            SelectedEmail = selectedPartner.Email;
            SelectedAddress = selectedPartner.Partner.Address;
            SelectedCity= selectedPartner.Partner.City;
            SelectedPostalCode= selectedPartner.Partner.PostalCode;
            SelectedBirthdate = selectedPartner.Partner.Birthdate;
        }

        public async Task Edit()
        {
            UserPartnerModel partner = _mapper.Map<UserPartnerModel>(SelectedPartner);

            // Below lines are USEFUL for sending data to partnerEndPoint
            partner.FirstName = SelectedFirstName;
            partner.LastName = SelectedLastName;
            partner.Partner.Phone = SelectedPhone;
            partner.Email = SelectedEmail;
            partner.Partner.Address = SelectedAddress;
            partner.Partner.City = SelectedCity;
            partner.Partner.PostalCode = SelectedPostalCode;
            partner.Partner.Birthdate = SelectedBirthdate;

            // Below lines are USEFUL for INotifyPropertyChange in UserPartnerDisplayModel
            // and in PartnerDisplayModel
            SelectedPartner.FirstName = SelectedFirstName;
            SelectedPartner.LastName = SelectedLastName;
            SelectedPartner.Email = SelectedEmail;
            SelectedPartner.Partner.Phone = SelectedPhone;
            SelectedPartner.Partner.Address = SelectedAddress;
            SelectedPartner.Partner.PostalCode = SelectedPostalCode;
            SelectedPartner.Partner.City = SelectedCity;
            SelectedPartner.Partner.Birthdate = SelectedBirthdate;

            await _partnerEndpoint.UpdatePartner(partner);
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
