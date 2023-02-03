using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using Newtonsoft.Json.Linq;
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

        public PartnerDetailsViewModel(IPartnerEndpoint partnerEndpoint, IMapper mapper)
        {
            _partnerEndpoint = partnerEndpoint;
            _mapper = mapper;
            _categories = new Dictionary<int, string>();
        }

        // Categories: needed when you select a category to change in the combobox (AvailableCategories), 
        // we will need to get the Id of the corresponding categoryName from this Dictionary
        private Dictionary<int, string> _categories;

        public Dictionary<int, string> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }

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

        private int _selectedIdCategory;

        public int SelectedIdCategory
        {
            get { return _selectedIdCategory; }
            set
            {
                _selectedIdCategory = value;
                NotifyOfPropertyChange(() => SelectedIdCategory);
            }
        }

        private string _selectedCategoryName;

        public string SelectedCategoryName
        {
            get { return _selectedCategoryName; }
            set
            {
                _selectedCategoryName = value;
                NotifyOfPropertyChange(() => SelectedCategoryName);
            }
        }

        private string _selectedAvailableCategory;
        public string SelectedAvailableCategory
        {
            get { return _selectedAvailableCategory; }
            set
            {
                _selectedAvailableCategory = value;
                SelectedCategoryName = value;
                NotifyOfPropertyChange(() => SelectedAvailableCategory);
                ChangeSelectedCategory();
            }
        }

        private string _selectedIBAN;

        public string SelectedIBAN
        {
            get { return _selectedIBAN; }
            set
            {
                _selectedIBAN = value;
                NotifyOfPropertyChange(() => SelectedIBAN);
            }
        }

        private string _selectedSIRET;

        public string SelectedSIRET
        {
            get { return _selectedSIRET; }
            set
            {
                _selectedSIRET = value;
                NotifyOfPropertyChange(() => SelectedSIRET);
            }
        }

        // AvailableCategories for the combobox
        private BindingList<string> _availableCategories = new BindingList<string>();

        public BindingList<string> AvailableCategories
        {
            get { return _availableCategories; }
            set 
            {
                _availableCategories = value;
                NotifyOfPropertyChange(() => AvailableCategories);
            }
        }

        private void ChangeSelectedCategory()
        {
            foreach (KeyValuePair<int, string> category in Categories)
            {
                if(SelectedCategoryName.Equals(category.Value))
                {
                    SelectedIdCategory = category.Key;
                }
            }
        }

        // Load the categories displayed in the combobox
        private async Task LoadCategories()
        {
            // If the Dictionary of categories is null: add all categories
            // The list of categories will be null if a modal is opened for the first time
            if (Categories == null || Categories.Count < 1)
            {
                var categories = await _partnerEndpoint.GetAllCategories();
                foreach (var category in categories)
                {
                    AvailableCategories.Add(category.Name);
                    Categories.Add(category.Id, category.Name);
                }
            }   
        }

        // Call this method when you click on a partner
        public async Task UpdatePartnerDetails(UserPartnerDisplayModel selectedPartner)
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
            SelectedIdCategory= selectedPartner.Partner.IdCategory;
            SelectedCategoryName = selectedPartner.Partner.CategoryName;
            SelectedAvailableCategory = selectedPartner.Partner.CategoryName;
            SelectedSIRET = selectedPartner.Partner.SIRET;
            SelectedIBAN = selectedPartner.Partner.IBAN;

            // Load the categories displayed in the combobox
            await LoadCategories();
        }
        public async Task Add()
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
            partner.Partner.IdCategory = SelectedIdCategory;
            partner.Partner.CategoryName = SelectedCategoryName;
            partner.Partner.SIRET = SelectedSIRET;
            partner.Partner.IBAN = SelectedIBAN;
    
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
            SelectedPartner.Partner.IdCategory = SelectedIdCategory;
            SelectedPartner.Partner.CategoryName = SelectedCategoryName;
            SelectedPartner.Partner.SIRET = SelectedSIRET;
            SelectedPartner.Partner.IBAN = SelectedIBAN;

            await _partnerEndpoint.AddPartner(partner);
            Close();
        }

        // Call this method when you submit the edit form of a partner
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
            partner.Partner.IdCategory = SelectedIdCategory;
            partner.Partner.CategoryName = SelectedCategoryName;
            partner.Partner.SIRET = SelectedSIRET;
            partner.Partner.IBAN = SelectedIBAN;

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
            SelectedPartner.Partner.IdCategory = SelectedIdCategory;
            SelectedPartner.Partner.CategoryName = SelectedCategoryName;
            SelectedPartner.Partner.SIRET = SelectedSIRET;
            SelectedPartner.Partner.IBAN = SelectedIBAN;

            await _partnerEndpoint.UpdatePartner(partner);
            Close();
        }

        public async Task Delete()
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
            partner.Partner.IdCategory = SelectedIdCategory;
            partner.Partner.CategoryName = SelectedCategoryName;

            // Below lines are USEFUL for INotifyPropertyChange in UserPartnerDisplayModel
            // and in PartnerDisplayModel
            SelectedPartner.IsActive = false;

            await _partnerEndpoint.DeletePartner(partner);
            Close();
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
