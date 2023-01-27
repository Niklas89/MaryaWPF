using AutoMapper;
using Caliburn.Micro;
using LiveCharts.Wpf;
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
    public class ClientDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        IMapper _mapper;
        private ClientDetailsViewModel _clientDetails;
        private AddClientViewModel _addClient;
        private readonly IWindowManager _window;
        IClientEndpoint _clientEndpoint;
        private BindingList<UserClientDisplayModel> _clients;

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

        private ClientDisplayModel _newClient;
        public ClientDisplayModel NewClient
        {
            get { return _newClient; }
            set
            {
                _newClient = value;
            }
        }

        private UserClientDisplayModel _newUserClient;
        public UserClientDisplayModel NewUserClient
        {
            get { return _newUserClient; }
            set
            {
                _newUserClient = value;
            }
        }

        public ClientDisplayViewModel(ClientDetailsViewModel clientDetails, AddClientViewModel addClient, IMapper mapper, StatusInfoViewModel status, IWindowManager window, IClientEndpoint clientEndpoint)
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
        }

        // Wait before the View loads
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadClients();
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

        private async Task LoadClients()
        {
            var clientList = await _clientEndpoint.GetAll();
            var clients = _mapper.Map<List<UserClientDisplayModel>>(clientList);
            Clients = new BindingList<UserClientDisplayModel>(clients);
        }

        private UserClientDisplayModel _selectedClient;

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

        private int _selectedClientId;

        public int SelectedClientId
        {
            get { return _selectedClientId; }
            set
            {
                _selectedClientId = value;
                NotifyOfPropertyChange(() => SelectedClientId);
            }
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
    }
}


