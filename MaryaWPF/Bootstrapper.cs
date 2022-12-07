using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Helpers;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using MaryaWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MaryaWPF
{
    public class Bootstrapper : BootstrapperBase // [Caliburn.Mirco] - Configuring, and implementing SimpleContainer for Desktop-app
    {
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
            PasswordBoxHelper.BoundPasswordProperty,
            "Password",
            "PasswordChanged");
        }

        private IMapper ConfigureAutomapper()
        {
            // AutoMapper nuget: map from source to destination model
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BookingModel, BookingDisplayModel>();
                cfg.CreateMap<PartnerModel, PartnerDisplayModel>();
                cfg.CreateMap<UserPartnerModel, UserPartnerDisplayModel>();
                // cfg.CreateMap<sourceModel, destinationModel>();
            });

            var output = config.CreateMapper();

            return output;
        }

        protected override void Configure()
        {
            _container.Instance(ConfigureAutomapper());

            // whenever we ask for SimpleContainer the configuration makes sure to get back the instance of _container
            _container.Instance(_container)
            .PerRequest<IBookingEndpoint, BookingEndpoint>()
            .PerRequest<IServiceEndpoint, ServiceEndpoint>()
            .PerRequest<IClientEndpoint, ClientEndpoint>()
            .PerRequest<IPartnerEndpoint, PartnerEndpoint>();

            _container
                .Singleton<IWindowManager, WindowManager>() // Singleton : create one instance for the life of the application / for the scope of the container
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>() // everytime we ask for ILoggedInUserModel we get back the same instance used in the whole application
                .Singleton<IAPIHelper, APIHelper>();


            /*
            _container // ICalculations instanciated in ShellViewModel.cs
                .PerRequest<ICalculations, Calculations>(); // return Calculations class per request (5 instances will be created if 5 request are made, not like Singleton)
            */

            GetType().Assembly.GetTypes() // get every type in the whole application
                .Where(type => type.IsClass) // only where the type is a class
                .Where(type => type.Name.EndsWith("ViewModel")) // where type ends with "ViewModel"
                .ToList() // returns an IEnumerable, so we need to transform into List so we can do Foreach
                .ForEach(viewModelType => _container.RegisterPerRequest( // Foreach on every class that ends with "ViewModel"
                    viewModelType, viewModelType.ToString(), viewModelType)); // Get a new instance everytime I request it, the type and the name
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            base.BuildUp(instance);
        }
    }
}
