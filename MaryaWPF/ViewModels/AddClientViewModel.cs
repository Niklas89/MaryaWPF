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
    public class AddClientViewModel : Screen
    {
        IClientEndpoint _clientEndpoint;
        IMapper _mapper;
        private UserClientDisplayModel _selectedClient;
        public UserClientDisplayModel SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                NotifyOfPropertyChange(() => SelectedClient);
            }
        }

        private BindingList<UserClientDisplayModel> _clients;

        public BindingList<UserClientDisplayModel> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                NotifyOfPropertyChange(() => Clients);
            }
        }

        public UserClientDisplayModel _newUserClient;

        public UserClientDisplayModel NewUserClient
        {
            get { return _newUserClient; }
            set
            {
                _newUserClient = value;
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

        public AddClientViewModel(IClientEndpoint clientEndpoint, IMapper mapper)
        {
            _clientEndpoint = clientEndpoint;
            _mapper = mapper;
            _newUserClient = new UserClientDisplayModel();
            _newClient = new ClientDisplayModel();
            _newUserClient.Client = _newClient;
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

        public async Task Add()
        {
            // Below lines are USEFUL for INotifyPropertyChange in UserPartnerDisplayModel
            // and in PartnerDisplayModel

            NewUserClient.FirstName = SelectedFirstName;
            NewUserClient.LastName = SelectedLastName;
            NewUserClient.Email = SelectedEmail;
            NewUserClient.Password = SelectedPassword;
            NewUserClient.Client.Phone = SelectedPhone;
            NewUserClient.Client.Address = SelectedAddress;
            NewUserClient.Client.PostalCode = SelectedPostalCode;
            NewUserClient.Client.City = SelectedCity;

            UserClientModel client = _mapper.Map<UserClientModel>(NewUserClient);

            // Below lines are USEFUL for sending data to partnerEndPoint
            client.FirstName = SelectedFirstName;
            client.LastName = SelectedLastName;
            client.Client.Phone = SelectedPhone;
            client.Email = SelectedEmail;
            client.Password = SelectedPassword;
            client.Client.Address = SelectedAddress;
            client.Client.City = SelectedCity;
            client.Client.PostalCode = SelectedPostalCode;

            await _clientEndpoint.AddClient(client);
            Close();
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
