﻿using AutoMapper;
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
    public class AddPartnerViewModel: Screen
    {
        IPartnerEndpoint _partnerEndpoint;
        IMapper _mapper;
        private UserPartnerDisplayModel _newUserPartner;
        public UserPartnerDisplayModel NewUserPartner
        {
            get { return _newUserPartner; }
            set
            {
                _newUserPartner = value;
            }
        }

        private PartnerDisplayModel _newPartner;
        public PartnerDisplayModel NewPartner
        {
            get { return _newPartner; }
            set
            {
                _newPartner = value;
            }
        }


        public AddPartnerViewModel(IPartnerEndpoint partnerEndpoint, IMapper mapper, UserPartnerDisplayModel partner)
        {
            _partnerEndpoint = partnerEndpoint;
            _mapper = mapper;
            _categories = new Dictionary<int, string>();
             _newUserPartner = new UserPartnerDisplayModel();
            _newPartner = new PartnerDisplayModel();
            _newUserPartner.Partner = _newPartner;

            _selectedPartner = partner;
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
        private string _selectedPassword;
        public string SelectedPassword
        {
            get { return _selectedPassword; }
            set
            {
                _selectedPassword = value;
                NotifyOfPropertyChange(() => SelectedPassword);
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
            set
            {
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
                if (SelectedCategoryName.Equals(category.Value))
                {
                    SelectedIdCategory = category.Key;
                }
            }
        }

        // Load the categories displayed in the combobox
        public async Task LoadCategories()
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

        public bool IsErrorVisible
        {
            get
            {
                bool output = false;

                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output;
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public void AddPartner(UserPartnerDisplayModel partner, BindingList<UserPartnerDisplayModel> partners)
        {
            ErrorMessage = "";
            _newUserPartner = partner;
            Partners = partners;
        }

        public async Task Add()
        {
            ErrorMessage = "";
            NewUserPartner.FirstName = SelectedFirstName;
            NewUserPartner.LastName = SelectedLastName;
            NewUserPartner.Email = SelectedEmail;
            NewUserPartner.Password = SelectedPassword;
            NewUserPartner.Partner.Phone = SelectedPhone;
            NewUserPartner.Partner.Address = SelectedAddress;
            NewUserPartner.Partner.PostalCode = SelectedPostalCode;
            NewUserPartner.Partner.City = SelectedCity;
            NewUserPartner.Partner.Birthdate = SelectedBirthdate;
            NewUserPartner.Partner.IdCategory = SelectedIdCategory;
            NewUserPartner.Partner.CategoryName = SelectedCategoryName;
            NewUserPartner.Partner.IBAN = SelectedIBAN;
            NewUserPartner.Partner.SIRET = SelectedSIRET;

            UserPartnerModel partner = _mapper.Map<UserPartnerModel>(NewUserPartner);

            try
            {
                // Below lines are USEFUL for sending data to serviceEndPoint

                partner.FirstName = SelectedFirstName;
                partner.LastName = SelectedLastName;
                partner.Partner.Phone = SelectedPhone;
                partner.Email = SelectedEmail;
                partner.Password = SelectedPassword;
                partner.Partner.Address = SelectedAddress;
                partner.Partner.City = SelectedCity;
                partner.Partner.PostalCode = SelectedPostalCode;
                partner.Partner.Birthdate = SelectedBirthdate;
                partner.Partner.IdCategory = SelectedIdCategory;
                partner.Partner.CategoryName = SelectedCategoryName;
                partner.Partner.IBAN = SelectedIBAN;
                partner.Partner.SIRET = SelectedSIRET;

                await _partnerEndpoint.AddPartner(partner);

                // After Add: get the last inserted service and insert it in the list bound to the datagrid
                await LoadPartnersAfterAdd();

                SelectedFirstName = null;
                SelectedLastName = null;
                SelectedEmail = null;
                SelectedPassword = null;
                SelectedPhone = null;
                SelectedAddress = null;
                SelectedCity = null;
                SelectedBirthdate = null;
                SelectedPostalCode = null;
                SelectedIBAN = null;
                SelectedSIRET = null;

                Close();

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

        }

        private async Task LoadPartnersAfterAdd()
        {
            var partnerListAfterAdd = await _partnerEndpoint.GetAll();
            var partnersAfterAdd = _mapper.Map<List<UserPartnerDisplayModel>>(partnerListAfterAdd);
            UserPartnerDisplayModel lastInsertPartner = partnersAfterAdd.LastOrDefault();
            lastInsertPartner.FirstName = SelectedFirstName;
            lastInsertPartner.LastName = SelectedLastName;
            lastInsertPartner.Email = SelectedEmail;
            lastInsertPartner.Password = SelectedPassword;
            lastInsertPartner.Partner.Phone = SelectedPhone;
            lastInsertPartner.Partner.Address = SelectedAddress;
            lastInsertPartner.Partner.City = SelectedCity;
            lastInsertPartner.Partner.PostalCode = SelectedPostalCode;
            lastInsertPartner.Partner.Birthdate = SelectedBirthdate;
            lastInsertPartner.Partner.IdCategory = SelectedIdCategory;
            lastInsertPartner.Partner.CategoryName = SelectedCategoryName;
            lastInsertPartner.Partner.IBAN = SelectedIBAN;
            lastInsertPartner.Partner.SIRET = SelectedSIRET;
            Partners.Add(lastInsertPartner);
        }
        //public async Task Add()
        //{
        //    // Below lines are USEFUL for INotifyPropertyChange in UserPartnerDisplayModel
        //    // and in PartnerDisplayModel

        //    NewUserPartner.FirstName = SelectedFirstName;
        //    NewUserPartner.LastName = SelectedLastName;
        //    NewUserPartner.Email = SelectedEmail;
        //    NewUserPartner.Password = SelectedPassword;
        //    NewUserPartner.Partner.Phone = SelectedPhone;
        //    NewUserPartner.Partner.Address = SelectedAddress;
        //    NewUserPartner.Partner.PostalCode = SelectedPostalCode;
        //    NewUserPartner.Partner.City = SelectedCity;
        //    NewUserPartner.Partner.Birthdate = SelectedBirthdate;
        //    NewUserPartner.Partner.IdCategory = SelectedIdCategory;
        //    NewUserPartner.Partner.CategoryName = SelectedCategoryName;
        //    NewUserPartner.Partner.IBAN = SelectedIBAN;
        //    NewUserPartner.Partner.SIRET = SelectedSIRET;

        //    UserPartnerModel partner = _mapper.Map<UserPartnerModel>(NewUserPartner);

        //    // Below lines are USEFUL for sending data to partnerEndPoint
        //    partner.FirstName = SelectedFirstName;
        //    partner.LastName = SelectedLastName;
        //    partner.Partner.Phone = SelectedPhone;
        //    partner.Email = SelectedEmail;
        //    partner.Password = SelectedPassword;
        //    partner.Partner.Address = SelectedAddress;
        //    partner.Partner.City = SelectedCity;
        //    partner.Partner.PostalCode = SelectedPostalCode;
        //    partner.Partner.Birthdate = SelectedBirthdate;
        //    partner.Partner.IdCategory = SelectedIdCategory;
        //    partner.Partner.CategoryName = SelectedCategoryName;
        //    partner.Partner.IBAN = SelectedIBAN;
        //    partner.Partner.SIRET = SelectedSIRET;

        //    await _partnerEndpoint.AddPartner(partner);
        //    Close();
        //}

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
