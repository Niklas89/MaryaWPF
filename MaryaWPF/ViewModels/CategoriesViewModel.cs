using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using LiveCharts.Configurations;
using MaryaWPF.Library.Api;
using MaryaWPF.Models;

namespace MaryaWPF.ViewModels
{
    class CategoriesViewModel : Screen
    {
        IServiceEndpoint _categoriesEndpoint;
        IMapper _mapper;
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        //private ServiceDetailsViewModel _serviceDetails;

    public CategoriesViewModel(IServiceEndpoint categoriesEndpoint, IMapper mapper, StatusInfoViewModel status,
            IWindowManager window, ServiceDetailsViewModel serviceDetails)
    {
        _categoriesEndpoint = categoriesEndpoint;
        _mapper = mapper;
        _status = status;
        _categoriesEndpoint = categoriesEndpoint;
        _window = window;
    }

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

                // Get a new ViewModel each time we call ShowDialogAsync(),
                // have multiple copies of StatusInfoViewModel inside the same class
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
            var categorieList = await _categoriesEndpoint.GetAll();
            // AutoMapper nuget : link source model in MaryaWPF.Library to destination model in MaryaWPF
            var categorie = _mapper.Map<List<CategoriesDisplayModel>>(categorieList);
            Categories = new BindingList<CategoriesDisplayModel>(categorie);
        }

        private BindingList<CategoriesDisplayModel> _categories;

        public BindingList<CategoriesDisplayModel> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                NotifyOfPropertyChange(() => Categories);
            }
        }

        public object CategoriesEndpoint { get; private set; }
    }

}
