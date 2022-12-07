using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.ViewModels
{
    public class PartnerDetailsViewModel: Screen
    {
        public string Header { get; private set; }
        public string Message { get; private set; }

        public PartnerDetailsViewModel()
        {

        }

        public void UpdateMessage(string header, string message)
        {
            Header = header;
            Message = message;

            NotifyOfPropertyChange(() => Header);
            NotifyOfPropertyChange(() => Message);
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
