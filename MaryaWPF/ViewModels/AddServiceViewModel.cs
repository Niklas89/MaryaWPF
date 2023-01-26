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
    public class AddServiceViewModel : Screen
    {
        IServiceEndpoint _serviceEndpoint;
        IMapper _mapper;

        private ServiceDisplayModel _newService;
        public ServiceDisplayModel NewService
        {
            get { return _newService; }
            set
            {
                _newService = value;
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

        private string _serviceName;

        public string ServiceName
        {
            get { return _serviceName; }
            set
            {
                _serviceName = value;
            }
        }

        private float _price;

        public float Price
        {
            get { return _price; }
            set
            {
                _price = value;
            }
        }

        private int _selectedIdType;

        public int SelectedIdType
        {
            get { return _selectedIdType; }
            set
            {
                _selectedIdType = value;
            }
        }

        private string _selectedTypeName;

        public string SelectedTypeName
        {
            get { return _selectedTypeName; }
            set
            {
                _selectedTypeName = value;
            }
        }

        private string _priceId;

        public string PriceId
        {
            get { return _priceId; }
            set
            {
                _priceId = value;
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
                ChangeSelectedType();
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

        private string _selectedCategoryName;

        public string SelectedCategoryName
        {
            get { return _selectedCategoryName; }
            set
            {
                _selectedCategoryName = value;
            }
        }

        private int _selectedCategoryId;

        public int SelectedCategoryId
        {
            get { return _selectedCategoryId; }
            set
            {
                _selectedCategoryId = value;
            }
        }

        private Dictionary<int, string> _types;

        public Dictionary<int, string> Types
        {
            get { return _types; }
            set { _types = value; }
        }

        public AddServiceViewModel(IServiceEndpoint serviceEndpoint, IMapper mapper)
        {
            _serviceEndpoint = serviceEndpoint;
            _mapper = mapper;
            _types = new Dictionary<int, string>();
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

        public async void AddService(ServiceDisplayModel service, BindingList<ServiceDisplayModel> services)
        {
            ErrorMessage = "";
            _newService = service;
            Services = services;
            SelectedCategoryId = service.IdCategory;
            SelectedCategoryName = service.CategoryName;

            // Load the types displayed in the combobox
            await LoadTypes();
        }

        public async Task Add()
        {
            ErrorMessage = "";
            ServiceModel service = _mapper.Map<ServiceModel>(NewService);
            bool serviceAlreadyExist = false;
            bool priceAtZero = false;
            bool priceIdIsNull = false;
            bool typeIdAtZero = false;

            try
            {
                // Remove all non letter characters
                ServiceName = new string((from c in ServiceName
                                          where char.IsWhiteSpace(c) || char.IsLetterOrDigit(c)
                                           select c).ToArray());

                // Check if a service with the same name already exists
                foreach (var serv in Services)
                {
                    if (ServiceName.Equals(serv.Name))
                    {
                        serviceAlreadyExist = true;
                        throw new Exception();
                    }
                }

                if (Price < 1)
                {
                    priceAtZero = true;
                    throw new Exception();
                }

                if (string.IsNullOrEmpty(PriceId))
                {
                    priceIdIsNull = true;
                    throw new Exception();
                }

                if (SelectedIdType == 0)
                {
                    typeIdAtZero = true;
                    throw new Exception();
                }


                // Below lines are USEFUL for sending data to serviceEndPoint
                service.Name = ServiceName;
                service.Price = Price;
                service.IdType = SelectedIdType;
                service.IdCategory = SelectedCategoryId;
                service.PriceId = PriceId;

                // Add the new category
                await _serviceEndpoint.AddService(service);

                // Clear the View fields in case of another add
                ServiceName = null;
                Price = 0;
                PriceId = null;

                // After Add: get the last inserted service and insert it in the list bound to the datagrid
                await LoadServicesAfterAdd();

                Close();

            }
            catch(ArgumentNullException ex)
            {
                ErrorMessage = "Veuillez définir un nom pour le service à ajouter.";
            }
            catch (Exception ex)
            {
                if (serviceAlreadyExist)
                    ErrorMessage = "Le nom du service existe déjà. Veuillez en choisir une autre.";
                else if(priceAtZero)
                    ErrorMessage = "Le tarif pour un service doit être supérieur à 1 €.";
                else if(priceIdIsNull)
                    ErrorMessage = "Veuillez définir un ID du prix pour Stripe.";
                else if (typeIdAtZero)
                    ErrorMessage = "Veuillez choisir le Type pour le service à ajouter.";
                else
                    ErrorMessage = ex.Message;
            }

        }

        // After Add: get the last inserted service and insert it in the list bound to the datagrid
        private async Task LoadServicesAfterAdd()
        {
            var serviceListAfterAdd = await _serviceEndpoint.GetAllServicesByCategory(SelectedCategoryId);
            var servicesAfterAdd = _mapper.Map<List<ServiceDisplayModel>>(serviceListAfterAdd);
            ServiceDisplayModel lastInsertedService = servicesAfterAdd.LastOrDefault();
            lastInsertedService.CategoryName = SelectedCategoryName;
            lastInsertedService.TypeName = SelectedTypeName;
            Services.Add(lastInsertedService);
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
