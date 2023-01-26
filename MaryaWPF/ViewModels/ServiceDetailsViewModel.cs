using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaryaWPF.ViewModels
{
    public class ServiceDetailsViewModel : Screen
    {
        IServiceEndpoint _serviceEndpoint;
        IBookingEndpoint _bookingEndpoint;
        IMapper _mapper;

        private ServiceDisplayModel _selectedService;
        public ServiceDisplayModel SelectedService
        {
            get { return _selectedService; }
            set
            {
                _selectedService = value;
                NotifyOfPropertyChange(() => SelectedService);
            }
        }

        private BindingList<ServiceDisplayModel> _services;

        public BindingList<ServiceDisplayModel> Services
        {
            get { return _services; }
            set
            {
                _services = value;
                NotifyOfPropertyChange(() => Services);
            }
        }

        public ServiceDetailsViewModel(IServiceEndpoint serviceEndpoint, IBookingEndpoint bookingEndpoint,
            IMapper mapper)
        {
            _serviceEndpoint = serviceEndpoint;
            _bookingEndpoint = bookingEndpoint;
            _mapper = mapper;
            _types = new Dictionary<int, string>();
        }

        // Types: needed when you select a type to change in the combobox (AvailableTypes), 
        // we will need to get the Id of the corresponding typeName from this Dictionary
        private Dictionary<int, string> _types;

        public Dictionary<int, string> Types
        {
            get { return _types; }
            set { _types = value; }
        }

        private float _selectedPrice;

        public float SelectedPrice
        {
            get { return _selectedPrice; }
            set
            {
                _selectedPrice = value;
                NotifyOfPropertyChange(() => SelectedPrice);
            }
        }

        private string _selectedCategoryName;

        public string SelectedCategoryName
        {
            get { return _selectedCategoryName; }
            set
            {
                _selectedCategoryName = value;
            }
        }

        private string _selectedServiceName;

        public string SelectedServiceName
        {
            get { return _selectedServiceName; }
            set
            {
                _selectedServiceName = value;
            }
        }

        private int _selectedIdType;

        public int SelectedIdType
        {
            get { return _selectedIdType; }
            set
            {
                _selectedIdType = value;
                NotifyOfPropertyChange(() => SelectedIdType);
            }
        }

        private string _selectedTypeName;

        public string SelectedTypeName
        {
            get { return _selectedTypeName; }
            set
            {
                _selectedTypeName = value;
                NotifyOfPropertyChange(() => SelectedTypeName);
            }
        }

        private string _selectedPriceId;

        public string SelectedPriceId
        {
            get { return _selectedPriceId; }
            set
            {
                _selectedPriceId = value;
                NotifyOfPropertyChange(() => SelectedPriceId);
            }
        }

        private string _selectedAvailableType;

        public string SelectedAvailableType
        {
            get { return _selectedAvailableType; }
            set
            {
                _selectedAvailableType = value;
                SelectedTypeName = value;
                NotifyOfPropertyChange(() => SelectedAvailableType);
                ChangeSelectedType();
            }
        }

        public bool IsDeleteVisible
        {
            get
            {
                bool output = false;

                if (ServiceCanBeDeleted == true)
                {
                    output = true;
                }
                return output;
            }
        }

        private bool _serviceCanBeDeleted;

        public bool ServiceCanBeDeleted
        {
            get { return _serviceCanBeDeleted; }
            set
            {
                _serviceCanBeDeleted = value;
                NotifyOfPropertyChange(() => IsDeleteVisible);
                NotifyOfPropertyChange(() => ServiceCanBeDeleted);
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

        // AvailableTypes for the combobox
        private BindingList<string> _availableTypes = new BindingList<string>();

        public BindingList<string> AvailableTypes
        {
            get { return _availableTypes; }
            set
            {
                _availableTypes = value;
                NotifyOfPropertyChange(() => AvailableTypes);
            }
        }

        private void ChangeSelectedType()
        {
            foreach (KeyValuePair<int, string> type in Types)
            {
                if (SelectedTypeName.Equals(type.Value))
                {
                    SelectedIdType = type.Key;
                }
            }
        }

        // Check if the service exist in a booking so it can be deleted
        private async Task CheckIfServiceCanBeDeleted()
        {
            var bookingList = await _bookingEndpoint.GetAllBookingsByService(SelectedService.Id);
            if (bookingList.Count == 0) ServiceCanBeDeleted = true;
        }

        // Load the types displayed in the combobox
        private async Task LoadTypes()
        {
            // If the Dictionary of categories is null: add all categories
            // The list of categories will be null if a modal is opened for the first time
            if (Types == null || Types.Count < 1)
            {
                var types = await _serviceEndpoint.GetAllTypes();
                foreach (var type in types)
                {
                    AvailableTypes.Add(type.Name);
                    Types.Add(type.Id, type.Name);
                }
            }
        }

        public async void UpdateServiceDetails(ServiceDisplayModel selectedService, BindingList<ServiceDisplayModel> services)
        {
            _selectedService = selectedService;
            /*
            List<ServiceDisplayModel> serviceList = new List<ServiceDisplayModel>
            {
                selectedService
            };
            Services = new BindingList<ServiceDisplayModel>(serviceList); */
            Services = services;
            SelectedPrice = selectedService.Price;
            SelectedIdType = selectedService.IdType;
            SelectedTypeName = selectedService.TypeName;
            SelectedAvailableType = selectedService.TypeName;
            SelectedPriceId = selectedService.PriceId;
            SelectedCategoryName = selectedService.CategoryName;
            SelectedServiceName = selectedService.Name;

            // Load the types displayed in the combobox
            await LoadTypes();

            // Check if the service can be deleted
            await CheckIfServiceCanBeDeleted();

        }

        public async Task Edit()
        {

            ErrorMessage = "";
            ServiceModel service = _mapper.Map<ServiceModel>(SelectedService);
            bool priceAtZero = false;
            bool priceIdIsNull = false;
            bool typeIdAtZero = false;

            try
            {
                if (SelectedPrice < 1)
                {
                    priceAtZero = true;
                    throw new Exception();
                }

                if (string.IsNullOrEmpty(SelectedPriceId))
                {
                    priceIdIsNull = true;
                    throw new Exception();
                }

                if (SelectedIdType == 0)
                {
                    typeIdAtZero = true;
                    throw new Exception();
                }

                // Below lines are USEFUL for sending data to clientEndPoint
                service.Price = SelectedPrice;
                service.IdType = SelectedIdType;
                service.PriceId = SelectedPriceId;

                // Below lines are USEFUL for INotifyPropertyChange in ServiceDisplayModel
                SelectedService.Price = SelectedPrice;
                SelectedService.IdType = SelectedIdType;
                SelectedService.TypeName = SelectedTypeName;
                SelectedService.PriceId = SelectedPriceId;

                await _serviceEndpoint.UpdateService(service);

                Close();

            } catch(Exception ex)
            {
                if (priceAtZero)
                    ErrorMessage = "Le tarif pour un service doit être supérieur à 1 €.";
                else if (priceIdIsNull)
                    ErrorMessage = "Veuillez définir un ID du prix pour Stripe.";
                else if (typeIdAtZero)
                    ErrorMessage = "Veuillez choisir le Type pour le service à ajouter.";
                else
                    ErrorMessage = ex.Message;
            }
        }

        public async void Delete()
        {
            try
            {
                await _serviceEndpoint.DeleteService(SelectedService.Id);
                Services.Remove(SelectedService);
                Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
