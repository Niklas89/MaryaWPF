using Caliburn.Micro;
using MaryaWPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaryaWPF.Library.Api;
using MaryaWPF.EventModels;
using System.Threading;
using System.Windows.Input;
using System.Dynamic;
using System.Windows;

namespace MaryaWPF.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _email = "niklasedelstam@protonmail.com";
        private string _password = "Niklas89";
        private readonly IWindowManager _window;
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;
        private RegistrationViewModel _registration;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events, IWindowManager window, RegistrationViewModel registration)
        {
            _apiHelper = apiHelper;
            _events = events;
            _window = window;
            _registration = registration;
        }

        public string Email 
        { 
            get { return _email; } 
            set 
            { 
                _email = value;
                NotifyOfPropertyChange(() => Email);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool IsErrorVisible
        {
            get 
            {
                bool output = false;

                if(ErrorMessage?.Length > 0 ) 
                { 
                    output = true; 
                }   
                return output; 
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            { 
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }



        public string Password 
        {
            get { return _password; }
            set 
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        // CanLogIn : Can + name of method LogIn() which is linked to the button in View
        // The LogIn button will be disabled when password and email fields are empty
        public bool CanLogIn
        {
            get
            {
                bool output = false;

                if (Email?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }
                return output;
            }
        }

        public async Task LogIn()
        {
            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(Email, Password);

                // Capture more information about the user
                await _apiHelper.GetLoggedInUserInfo(result.AccessToken);

                // Say that someone logged in
                await _events.PublishOnUIThreadAsync(new LogOnEvent(), new CancellationToken());

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task Register()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            settings.Height = 600;
            settings.Width = 750;
            settings.SizeToContent = SizeToContent.Manual;
            settings.ResizeMode = ResizeMode.CanResize;
            settings.Title = "Inscription";

            await _window.ShowDialogAsync(_registration, null, settings);
        }

    }
}
