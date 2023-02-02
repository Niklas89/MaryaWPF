using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaryaWPF.ViewModels
{
    public class RegistrationViewModel : Screen
    {
        private string _email;
        private string _password;
        private string _firstName;
        private string _lastName;
        private readonly IMapper _mapper;
        private IWindowManager _window;
        private IProfileEndpoint _profileEndpoint;

        public RegistrationViewModel(IMapper mapper, IWindowManager window, IProfileEndpoint profileEndpoint)
        {
            _window = window;
            _mapper = mapper;
            _profileEndpoint = profileEndpoint;

        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(() => CanRegister);
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => CanRegister);
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
                NotifyOfPropertyChange(() => CanRegister);
            }
        }


        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanRegister);
            }
        }

        // CanRegister : Can + name of method Register() which is linked to the button in View
        // The Register button will be disabled when password and email fields are empty
        public bool CanRegister
        {
            get
            {
                bool output = false;

                if (FirstName?.Length > 0 && LastName?.Length > 0 && Email?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }
                return output;
            }
        }

        public bool IsErrorVisible
        {
            get
            {
                bool output = false;

                if (ErrorMessage?.Length > 0)
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

        public bool IsSuccessVisible
        {
            get
            {
                bool output = false;

                if (SuccessMessage?.Length > 0)
                {
                    output = true;
                }
                return output;
            }
        }

        private string _successMessage;

        public string SuccessMessage
        {
            get { return _successMessage; }
            set
            {
                _successMessage = value;
                NotifyOfPropertyChange(() => IsSuccessVisible);
                NotifyOfPropertyChange(() => SuccessMessage);
            }
        }


        public async void Register()
        {
            ErrorMessage = "";
            SuccessMessage = "";
            bool registerUserSuccess = false;

            try
            {
                if (!IsValidEmail(Email))
                    throw new FormatException();

                if (string.IsNullOrEmpty(FirstName))
                    throw new ArgumentNullException();

                if (string.IsNullOrEmpty(LastName))
                    throw new ArgumentNullException();

                if (string.IsNullOrEmpty(Password))
                    throw new ArgumentNullException();

                registerUserSuccess = await _profileEndpoint.RegisterUser(FirstName, LastName, Email, Password);
            }
            catch (FormatException ex)
            {
                ErrorMessage = "Vous n'avez pas indiqué une adresse email au bon format.";
            }
            catch (ArgumentNullException ex)
            {
                ErrorMessage = "Vous devez remplir tous les champs.";
            }
            catch (Exception ex)
            {
                    ErrorMessage = ex.Message;
            }
            finally
            {
                if (registerUserSuccess)
                {
                    SuccessMessage = "Votre compte a été créé avec succès !";
                    Password = "";
                }

            }
        }

        public bool IsValidEmail(string email)
        {
            if (!MailAddress.TryCreate(email, out var mailAddress))
                return false;

            // And if you want to be more strict:
            var hostParts = mailAddress.Host.Split('.');
            if (hostParts.Length == 1)
                return false; // No dot.
            if (hostParts.Any(p => p == string.Empty))
                return false; // Double dot.
            if (hostParts[^1].Length < 2)
                return false; // TLD only one letter.

            if (mailAddress.User.Contains(' '))
                return false;
            if (mailAddress.User.Split('.').Any(p => p == string.Empty))
                return false; // Double dot or dot at end of user part.

            return true;
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}