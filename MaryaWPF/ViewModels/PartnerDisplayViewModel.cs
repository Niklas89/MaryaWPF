﻿using AutoMapper;
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
    public class PartnerDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        IMapper _mapper;
        private PartnerDetailsViewModel _partnerDetails;
        private AddPartnerViewModel _addPartner;
        private readonly IWindowManager _window;
        IPartnerEndpoint _partnerEndpoint;
        IServiceEndpoint _serviceEndpoint;
        private BindingList<UserPartnerDisplayModel> _partners;

        public BindingList<UserPartnerDisplayModel> Partners
        {
            get
            {
                return _partners;
            }
            set
            {
                _partners = value;
                NotifyOfPropertyChange(() => Partners);
            }
        }

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

        public PartnerDisplayViewModel( PartnerDetailsViewModel partnerDetails, AddPartnerViewModel addPartner, StatusInfoViewModel status, IMapper mapper,
            IWindowManager window, IPartnerEndpoint partnerEndpoint, IServiceEndpoint serviceEndpoint)
        {
            _status = status;
            _mapper = mapper;
            _window = window;
            _partnerEndpoint = partnerEndpoint;
            _serviceEndpoint = serviceEndpoint;
            _partnerDetails = partnerDetails;
            _addPartner = addPartner;
            _newUserPartner = new UserPartnerDisplayModel();
            _newPartner = new PartnerDisplayModel();
            _newUserPartner.Partner = _newPartner;
        }

        // Wait before the View loads
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadPartners();
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

        private async Task LoadPartners()
        {
            var partnerList = await _partnerEndpoint.GetAll();
            var categoriesList = await _serviceEndpoint.GetAll();

            foreach(var partner in partnerList)
            {
                foreach(var category in categoriesList)
                {
                    if(partner.Partner.IdCategory == category.Id)
                    {
                        partner.Partner.CategoryName = category.Name;
                    }
                }
            }
            // AutoMapper nuget : link source model in MaryaWPF.Library to destination model in MaryaWPF
            var partners = _mapper.Map<List<UserPartnerDisplayModel>>(partnerList);
            var orderedPartners = partners.OrderBy(p => p.Email).ToList();
            Partners = new BindingList<UserPartnerDisplayModel>(orderedPartners);
        }

        private UserPartnerDisplayModel _selectedPartner;

        public UserPartnerDisplayModel SelectedPartner
        {
            get { return _selectedPartner; }
            set
            {
                _selectedPartner = value;
                NotifyOfPropertyChange(() => SelectedPartner);
                ViewPartnerDetails();
            }
        }

        public async void ViewPartnerDetails()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 750;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Détails du partenaire";

            await _partnerDetails.UpdatePartnerDetails(SelectedPartner);
            await _window.ShowDialogAsync(_partnerDetails, null, settings);
        }

        public async void ViewAddPartner()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 750;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Ajouter un partenaire";

            await _addPartner.LoadCategories();
            _addPartner.AddPartner(NewUserPartner, Partners);
            await _window.ShowDialogAsync(_addPartner, null, settings);
        }
    }
}
