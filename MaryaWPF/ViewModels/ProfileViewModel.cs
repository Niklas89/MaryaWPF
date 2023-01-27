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
    public class ProfileViewModel : Screen
    {
        private string _email;
        private string _password;
        private string _firstName;
        private string _lastName;
        private string _createdAt;
        private readonly StatusInfoViewModel _status;
        private readonly IMapper _mapper;
        private  IWindowManager _window;
        private  ILoggedInUserModel _loggedInUserModel;
        private IProfileEndpoint _profileEndpoint;

        public ProfileViewModel(IMapper mapper, StatusInfoViewModel status, IWindowManager window, ILoggedInUserModel loggedInUserModel,
            IProfileEndpoint profileEndpoint)
        {
            _status = status;
            _window = window;
            _mapper = mapper;
            _loggedInUserModel = loggedInUserModel;
            _profileEndpoint = profileEndpoint;

        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
            }
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

        public string CreatedAt
        {
            get { return _createdAt; }
            set
            {
                _createdAt = value;
                NotifyOfPropertyChange(() => CreatedAt);
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

        // Wait before the View loads
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                LoadProfile();
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

        private void LoadProfile()
        {
            FirstName = _loggedInUserModel.FirstName;
            LastName = _loggedInUserModel.LastName;
            CreatedAt = _loggedInUserModel.CreatedAt.ToString("dd/MM/yyyy");
            Email = _loggedInUserModel.Email;
        }

        public async void Edit()
        {
            ErrorMessage = "";
            SuccessMessage = "";
            int id;
            bool success = int.TryParse(_loggedInUserModel.Id, out id);
            bool updateProfileSuccess = false;

            try
            {
                if (!IsValidEmail(Email))
                    throw new FormatException();

                if(string.IsNullOrEmpty(Password))
                    throw new ArgumentNullException();

                if (success)
                    updateProfileSuccess = await _profileEndpoint.UpdateProfile(Email, Password);
                else
                    throw new Exception();

            } catch (FormatException ex)
            {
                ErrorMessage = "Vous n'avez pas indiqué une adresse email au bon format.";
            }
            catch(ArgumentNullException ex)
            {
                ErrorMessage = "Vous devez indiquer un mot de passe.";
            }
            catch(Exception ex)
            {
                if (!success)
                    ErrorMessage = "Votre ID n'a pas pu être converti en int";
                else
                    ErrorMessage = getBetween(ex.Message, "message:",",");
            } finally
            {
                if (updateProfileSuccess)
                    SuccessMessage = "Votre profil a été mis à jour avec succès !";
            }
        }

        public string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
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
    }
}