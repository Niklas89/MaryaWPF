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
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaryaWPF.ViewModels
{
    public class CategoryDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        IMapper _mapper;
        private ServiceDetailsViewModel _serviceDetails;
        private readonly IWindowManager _window;
        IServiceEndpoint _serviceEndpoint;

        BindingList<ServiceDisplayModel> _services;

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

        BindingList<CategoryDisplayModel> _categories;

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

        public CategoryDisplayViewModel(ServiceDetailsViewModel serviceDetails, IMapper mapper, StatusInfoViewModel status, IWindowManager window, IServiceEndpoint serviceEndpoint)
        {
            _status = status;
            _window = window;
            _mapper = mapper;
            _serviceEndpoint = serviceEndpoint;
            _serviceDetails = serviceDetails;
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
            Categories = new BindingList<CategoryDisplayModel>(categories);
        }

        private async void LoadServicesByCategory()
        {
            var serviceList = await _serviceEndpoint.GetAllServicesByCategory(SelectedCategoryId);
            var typeList = await _serviceEndpoint.GetAllTypes();

            var services = _mapper.Map<List<ServiceDisplayModel>>(serviceList);
            Services = new BindingList<ServiceDisplayModel>(services);

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
                SelectedServiceId = value.Id;
                NotifyOfPropertyChange(() => SelectedService);
                ViewServiceDetails();
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

        private CategoryDisplayModel _selectedCategory;

        public CategoryDisplayModel SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                _selectedCategory = value;
                SelectedCategoryId = value.Id;
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

        public async void ViewServiceDetails()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 750;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Détails du service";

            _serviceDetails.UpdateServiceDetails(SelectedService);
            await _window.ShowDialogAsync(_serviceDetails, null, settings);

        }

    }
}
