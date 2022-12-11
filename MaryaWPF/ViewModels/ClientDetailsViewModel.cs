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
    public class ClientDetailsViewModel : Screen
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

        public ClientDetailsViewModel(IClientEndpoint clientEndpoint, IMapper mapper)
        {
            _clientEndpoint = clientEndpoint;
            _mapper = mapper;
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

        public void UpdateClientDetails(UserClientDisplayModel selectedClient)
        {
            _selectedClient = selectedClient;
            List<UserClientDisplayModel> clientList = new List<UserClientDisplayModel>
            {
                selectedClient
            };
            Clients = new BindingList<UserClientDisplayModel>(clientList);
            SelectedFirstName = selectedClient.FirstName;
            SelectedLastName = selectedClient.LastName;
            SelectedEmail = selectedClient.Email;
            SelectedPhone = selectedClient.Client.Phone;
            SelectedAddress = selectedClient.Client.Address;
            SelectedPostalCode = selectedClient.Client.PostalCode;
            SelectedCity = selectedClient.Client.City;

        }

        public async Task Edit()
        {
            UserClientModel client = _mapper.Map<UserClientModel>(SelectedClient);
            
            // Below lines are USEFUL for sending data to clientEndPoint
            client.FirstName = SelectedFirstName;
            client.LastName = SelectedLastName;
            client.Email = SelectedEmail;
            client.Client.Phone = SelectedPhone;
            client.Client.Address = SelectedAddress;
            client.Client.PostalCode = SelectedPostalCode;
            client.Client.City = SelectedCity;

            // Below lines are USEFUL for INotifyPropertyChange in UserClientDisplayModel
            // and in ClientDisplayModel
            SelectedClient.FirstName = SelectedFirstName;
            SelectedClient.LastName = SelectedLastName;
            SelectedClient.Email = SelectedEmail;
            SelectedClient.Client.Phone = SelectedPhone;
            SelectedClient.Client.Address = SelectedAddress;
            SelectedClient.Client.PostalCode = SelectedPostalCode;
            SelectedClient.Client.City = SelectedCity;

            await _clientEndpoint.UpdateClient(client);
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
