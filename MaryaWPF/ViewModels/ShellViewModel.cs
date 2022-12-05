using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.EventModels;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using MaterialDesignThemes.Wpf;

namespace MaryaWPF.ViewModels
{
    // With Conductor only one item can be activated at a time
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>, IHandle<string>
    {
        // Test code for Dependency Injection with Bootstrapper
        /*
        private ICalculations _calculations;

        public ShellViewModel(ICalculations calculations)
        {
            _calculations = calculations;
        } */

        // private LoginViewModel _loginVM;
        private IEventAggregator _events;
        private ILoggedInUserModel _user;
        private IAPIHelper _apiHelper;
        private readonly IMapper _mapper;
        // private SimpleContainer _container;

        public ShellViewModel(IEventAggregator events, ILoggedInUserModel user, IAPIHelper apiHelper, IMapper mapper)
        {
            _events = events;
            // _loginVM = loginVM;
            _user = user;
            _apiHelper = apiHelper;
            //_container= container;
            _mapper = mapper;

            // send event to every subscriber, even if they aren't listening to that particular type:
            // Tell ShellViewModel to listen to LogOnEvent or string IHandle for example
            _events.SubscribeOnPublishedThread(this);

            // Activate LoginViewModel on ShellViewModel so I can use it later
            // when we active login this way we'll have our data from previously :
            // ActivateItemAsync(_loginVM); 
            // We can just ask for an instance of LoginViewModel that way it's fresh every single time (no data left from previously)
            // when we active login this way we won't have our data from previously :
            // ActivateItemAsync(_container.GetInstance<LoginViewModel>()); 
            // Easier way to do this with IoC (from Caliburn Micro) : allows us to talk to containers to get instances
            ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());

            LoadMenuItems();
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }
                return output;
            }
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }

        public async Task PartnerManagement()
        {
            await ActivateItemAsync(IoC.Get<PartnerDisplayViewModel>(), new CancellationToken());
        }

        public async Task BookingManagement()
        {
            await ActivateItemAsync(IoC.Get<DashboardViewModel>(), new CancellationToken());
        }

        public async Task ClientManagement()
        {
            await ActivateItemAsync(IoC.Get<ClientDisplayViewModel>(), new CancellationToken());
        }

        public async Task LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOffUser();
            await ActivateItemAsync(IoC.Get<LoginViewModel>(), new CancellationToken());

            // Trigger at logout
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            // Activate Dashboard and close Login, because with Conductor<object> only one item can be activated at a time
            await ActivateItemAsync(IoC.Get<DashboardViewModel>(), cancellationToken);

            // Get a new instance of LoginViewModel and place it inside _loginVM, otherwise we'll still have our sensitive information in it (email and pass)
            // _loginVM = _container.GetInstance<LoginViewModel>(); 

            // Trigger at login
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        // Example can be deleted - with IHandle<string>
        public Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        // Menu
        private void LoadMenuItems()
        {
            List<ShellDisplayModel> menuList = new()
            {
                new ShellDisplayModel
                {
                    Title = "Payment",
                    SelectedIcon = PackIconKind.CreditCard,
                    UnselectedIcon = PackIconKind.CreditCardOutline,
                    Notification = 1
                },
                new ShellDisplayModel
                {
                    Title = "Home",
                    SelectedIcon = PackIconKind.Home,
                    UnselectedIcon = PackIconKind.HomeOutline,
                },
                new ShellDisplayModel
                {
                    Title = "Special",
                    SelectedIcon = PackIconKind.Star,
                    UnselectedIcon = PackIconKind.StarOutline,
                },
                new ShellDisplayModel
                {
                    Title = "Shared",
                    SelectedIcon = PackIconKind.Users,
                    UnselectedIcon = PackIconKind.UsersOutline,
                },
                new ShellDisplayModel
                {
                    Title = "Files",
                    SelectedIcon = PackIconKind.Folder,
                    UnselectedIcon = PackIconKind.FolderOutline,
                },
                new ShellDisplayModel
                {
                    Title = "Library",
                    SelectedIcon = PackIconKind.Bookshelf,
                    UnselectedIcon = PackIconKind.Bookshelf,
                },
            };
            var items = _mapper.Map<List<ShellDisplayModel>>(menuList);
            MenuItems = new BindingList<ShellDisplayModel>(items);
        }

        private BindingList<ShellDisplayModel> _menuItems;

        public BindingList<ShellDisplayModel> MenuItems
        {
            get { return _menuItems; }
            set
            {
                _menuItems = value;
                NotifyOfPropertyChange(() => _menuItems);
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        => _menuItems[0].Notification = _menuItems[0].Notification is null ? 1 : null;

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
            => _menuItems[0].Notification = _menuItems[0].Notification is null ? "123+" : null;
    }
}
