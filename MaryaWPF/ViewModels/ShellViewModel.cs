﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MaryaWPF.EventModels;

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
        private DashboardViewModel _dashboardVM;
        // private SimpleContainer _container;

        public ShellViewModel(IEventAggregator events, DashboardViewModel dashboardVM)
        {
            _events = events;
            // _loginVM = loginVM;
            _dashboardVM = dashboardVM;
            //_container= container;

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
            ActivateItemAsync(IoC.Get<LoginViewModel>()); 
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }

        public void LogOut()
        {

        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            // Activate Dashboard and close Login, because with Conductor<object> only one item can be activated at a time
            await ActivateItemAsync(_dashboardVM, cancellationToken);

            // Get a new instance of LoginViewModel and place it inside _loginVM, otherwise we'll still have our sensitive information in it (email and pass)
           // _loginVM = _container.GetInstance<LoginViewModel>(); 
        }

        // Example can be deleted - with IHandle<string>
        public Task HandleAsync(string message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}