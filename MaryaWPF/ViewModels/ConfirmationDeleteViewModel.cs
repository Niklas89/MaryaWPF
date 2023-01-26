using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.ViewModels
{
    public class ConfirmationDeleteViewModel : Screen
    {
        public string Header { get; private set; }
        public string Message { get; private set; }

        private string _confirmationToDelete;

        public string ConfirmationToDelete
        {
            get { return _confirmationToDelete; }
            set
            {
                _confirmationToDelete = value;
                NotifyOfPropertyChange(() => ConfirmationToDelete);
            }
        }

        public void UpdateMessage(string header, string message, string confirmationToDelete)
        {
            Header = header;
            Message = message;
            ConfirmationToDelete = confirmationToDelete;

            NotifyOfPropertyChange(() => Header);
            NotifyOfPropertyChange(() => Message);
        }

        public void Delete()
        {
            ConfirmationToDelete += "Delete";
            Close();
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
