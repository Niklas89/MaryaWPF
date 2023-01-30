using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
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
    public class RecrutedClientsPartnersViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        IMapper _mapper;
        private readonly IWindowManager _window;
        private readonly IClientEndpoint _clientEndpoint;
        private readonly IPartnerEndpoint _partnerEndpoint;
        private readonly IServiceEndpoint _serviceEndpoint;

        private ClientDetailsViewModel _clientDetails;
        private AddClientViewModel _addClient;
        private BindingList<UserClientDisplayModel> _clients;
        private ClientDisplayModel _newClient;
        private UserClientDisplayModel _newUserClient;
        private UserClientDisplayModel _selectedClient;
        private int _selectedClientId;

        private PartnerDetailsViewModel _partnerDetails;
        private AddPartnerViewModel _addPartner;
        private BindingList<UserPartnerDisplayModel> _partners;
        private UserPartnerDisplayModel _newUserPartner;
        private PartnerDisplayModel _newPartner;
        private UserPartnerDisplayModel _selectedPartner;


        public RecrutedClientsPartnersViewModel(ClientDetailsViewModel clientDetails, AddClientViewModel addClient,
            PartnerDetailsViewModel partnerDetails, AddPartnerViewModel addPartner,
            IMapper mapper, StatusInfoViewModel status, IWindowManager window, IClientEndpoint clientEndpoint,
            IPartnerEndpoint partnerEndpoint, IServiceEndpoint serviceEndpoint)
        {
            _status = status;
            _window = window;
            _mapper = mapper;
            _newUserClient = new UserClientDisplayModel();
            _newClient = new ClientDisplayModel();
            _newUserClient.Client = _newClient;
            _clientEndpoint = clientEndpoint;
            _clientDetails = clientDetails;
            _addClient = addClient;

            _partnerEndpoint = partnerEndpoint;
            _serviceEndpoint = serviceEndpoint;
            _partnerDetails = partnerDetails;
            _addPartner = addPartner;
            _newUserPartner = new UserPartnerDisplayModel();
            _newPartner = new PartnerDisplayModel();
            _newUserPartner.Partner = _newPartner;
        }

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


        public UserPartnerDisplayModel NewUserPartner
        {
            get { return _newUserPartner; }
            set
            {
                _newUserPartner = value;
            }
        }


        public PartnerDisplayModel NewPartner
        {
            get { return _newPartner; }
            set
            {
                _newPartner = value;
            }
        }


        public BindingList<UserClientDisplayModel> Clients
        {
            get
            {
                return _clients;
            }
            set
            {
                _clients = value;
                NotifyOfPropertyChange(() => Clients);
            }
        }


        public ClientDisplayModel NewClient
        {
            get { return _newClient; }
            set
            {
                _newClient = value;
            }
        }


        public UserClientDisplayModel NewUserClient
        {
            get { return _newUserClient; }
            set
            {
                _newUserClient = value;
            }
        }


        public UserClientDisplayModel SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                SelectedClientId = value.Id;
                NotifyOfPropertyChange(() => SelectedClient);
                ViewClientDetails();
            }
        }


        public int SelectedClientId
        {
            get { return _selectedClientId; }
            set
            {
                _selectedClientId = value;
                NotifyOfPropertyChange(() => SelectedClientId);
            }
        }

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

        // Wait before the View loads
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadClients();
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
            var partnerList = await _partnerEndpoint.GetAllRecruted();
            var categoriesList = await _serviceEndpoint.GetAll();

            foreach (var partner in partnerList)
            {
                foreach (var category in categoriesList)
                {
                    if (partner.Partner.IdCategory == category.Id)
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
        

        private async Task LoadClients()
        {
            var clientList = await _clientEndpoint.GetAllRecruted();
            var clients = _mapper.Map<List<UserClientDisplayModel>>(clientList);
            var orderedClients = clients.OrderBy(p => p.Email).ToList();
            Clients = new BindingList<UserClientDisplayModel>(orderedClients);
        }

        public async void ViewClientDetails()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 750;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Détails du client";

            _clientDetails.UpdateClientDetails(SelectedClient);
            await _window.ShowDialogAsync(_clientDetails, null, settings);

        }

        public async void ViewAddClient()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 750;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Ajouter un client";

            _addClient.AddClient(NewUserClient, Clients);
            await _window.ShowDialogAsync(_addClient, null, settings);
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
