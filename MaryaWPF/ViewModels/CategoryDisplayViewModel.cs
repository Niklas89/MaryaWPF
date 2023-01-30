using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaryaWPF.ViewModels
{
    public class CategoryDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private ServiceDetailsViewModel _serviceDetails;
        private AddCategoryViewModel _addCategory;
        private AddServiceViewModel _addService;
        private readonly IWindowManager _window;
        IServiceEndpoint _serviceEndpoint;
        IMapper _mapper;

        private BindingList<ServiceDisplayModel> _services;

        public BindingList<ServiceDisplayModel> Services
        {
            get
            {
                return _services;
            }
            set
            {
                _services = value;
                NotifyOfPropertyChange(() => Services);
            }
        }

        private BindingList<CategoryDisplayModel> _categories;

        public BindingList<CategoryDisplayModel> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                NotifyOfPropertyChange(() => Categories);
            }
        }

        private CategoryDisplayModel _newCategory;
        public CategoryDisplayModel NewCategory
        {
            get { return _newCategory; }
            set
            {
                _newCategory = value;
            }
        }

        private ServiceDisplayModel _newService;
        public ServiceDisplayModel NewService
        {
            get { return _newService; }
            set
            {
                _newService = value;
            }
        }

        public CategoryDisplayViewModel(ServiceDetailsViewModel serviceDetails, AddCategoryViewModel addCategory, AddServiceViewModel addService,
            IMapper mapper, StatusInfoViewModel status, IWindowManager window, IServiceEndpoint serviceEndpoint)
        {
            _status = status;
            _window = window;
            _mapper = mapper;
            _serviceEndpoint = serviceEndpoint;
            _serviceDetails = serviceDetails;
            _newCategory = new CategoryDisplayModel();
            _newService = new ServiceDisplayModel();
            _addCategory = addCategory;
            _addService= addService;
        }

        // Wait before the View loads
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadCategories();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "Erreur Système";

                var info = IoC.Get<StatusInfoViewModel>();

                if (ex.Message == "Forbidden")
                {
                    _status.UpdateMessage("Accès refusé", "Vous n'avez pas l'autorisation de voir les réservations sur l'application bureautique.");
                    await _window.ShowDialogAsync(_status, null, settings);
                }
                else
                {
                    _status.UpdateMessage("Fatal Exception", ex.Message);
                    await _window.ShowDialogAsync(_status, null, settings);
                }

                await TryCloseAsync();
            }
        }

        private async Task LoadCategories()
        {
            var categoryList = await _serviceEndpoint.GetAll();
            var categories = _mapper.Map<List<CategoryDisplayModel>>(categoryList);
            var orderedCategories = categories.OrderBy(c => c.Name).ToList();
            Categories = new BindingList<CategoryDisplayModel>(orderedCategories);
        }

        private async void LoadServicesByCategory()
        {
            var serviceList = await _serviceEndpoint.GetAllServicesByCategory(SelectedCategoryId);
            var typeList = await _serviceEndpoint.GetAllTypes();

            var services = _mapper.Map<List<ServiceDisplayModel>>(serviceList);
            var orderedServices = services.OrderBy(c => c.Name).ToList();
            Services = new BindingList<ServiceDisplayModel>(orderedServices);

            foreach (var service in Services)
            {
                foreach (var category in Categories)
                {
                    if (service.IdCategory == category.Id)
                    {
                        service.CategoryName = category.Name;
                    }
                }

                foreach (var type in typeList)
                {
                    if (service.IdType == type.Id)
                    {
                        service.TypeName = type.Name;
                    }
                }
            }
        }

        private ServiceDisplayModel _selectedService;

        public ServiceDisplayModel SelectedService
        {
            get { return _selectedService; }
            set
            {
                    _selectedService = value;
                    NotifyOfPropertyChange(() => SelectedService);
                    ViewServiceDetails(_selectedService);
            }
        }

        private int _selectedServiceId;

        public int SelectedServiceId
        {
            get { return _selectedServiceId; }
            set
            {
                _selectedServiceId = value;
                NotifyOfPropertyChange(() => SelectedServiceId);
            }
        }

        public bool IsSelectedCategory
        {
            get
            {
                bool output = false;

                if (SelectedCategory != null)
                {
                    output = true;
                }
                return output;
            }
        }

        private CategoryDisplayModel _selectedCategory;

        public CategoryDisplayModel SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                SelectedCategoryId = value.Id;
                NotifyOfPropertyChange(() => IsSelectedCategory);
                NotifyOfPropertyChange(() => SelectedCategory);
                LoadServicesByCategory();
            }
        }

        private int _selectedCategoryId;

        public int SelectedCategoryId
        {
            get { return _selectedCategoryId; }
            set
            {
                _selectedCategoryId = value;
                NotifyOfPropertyChange(() => SelectedCategoryId);
            }
        }

        public async void ViewServiceDetails(ServiceDisplayModel selectedService)
        {
            if(selectedService != null)
            {
                SelectedServiceId = selectedService.Id;

                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.Height = 400;
                settings.Width = 750;
                settings.SizeToContent = SizeToContent.Manual;
                settings.ResizeMode = ResizeMode.CanResize;
                settings.Title = "Détails du service";

                _serviceDetails.UpdateServiceDetails(SelectedService, Services);
                await _window.ShowDialogAsync(_serviceDetails, null, settings);
            }
        }

        public async void AddCategory()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 400;
            settings.Width = 750;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Ajout d'une catégorie";

            _addCategory.AddCategory(NewCategory, Categories);
            await _window.ShowDialogAsync(_addCategory, null, settings);
        }

        public async void AddService()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 750;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Ajout d'un service";

            NewService.IdCategory = SelectedCategoryId;
            NewService.CategoryName = SelectedCategory.Name;

            _addService.AddService(NewService, Services);
            await _window.ShowDialogAsync(_addService, null, settings);
        }

    }
}
