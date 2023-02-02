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
    public class ResetPasswordViewModel : Screen
    {
        private string _email;
        private readonly IMapper _mapper;
        private IWindowManager _window;
        private IProfileEndpoint _profileEndpoint;

        public ResetPasswordViewModel(IMapper mapper, IWindowManager window, IProfileEndpoint profileEndpoint)
        {
            _window = window;
            _mapper = mapper;
            _profileEndpoint = profileEndpoint;

        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
                NotifyOfPropertyChange(() => CanSend);
            }
        }


        // CanSend : Can + name of method Send() which is linked to the button in View
        // The Send button will be disabled when email field is empty
        public bool CanSend
        {
            get
            {
                bool output = false;

                if (Email?.Length > 0)
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


        public async void Send()
        {
            ErrorMessage = "";
            SuccessMessage = "";
            bool resetPasswordSuccess = false;

            try
            {
                if (!IsValidEmail(Email))
                    throw new FormatException();

                resetPasswordSuccess = await _profileEndpoint.ResetPassword(Email);
            }
            catch (FormatException ex)
            {
                ErrorMessage = "Vous n'avez pas indiqué une adresse email au bon format.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                if (resetPasswordSuccess)
                {
                    SuccessMessage = "Félicitations, l'email a bien été envoyé." +
                        "Vous pouvez à présent définir votre nouveau mot de passe en cliquant sur le lien dans l'email " +
                        "qui vous a été envoyé.";
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