using MaryaWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaryaWPF.Commands
{
    public class HomeButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        ShellViewModel _buttonViewModel;

        public HomeButtonCommand(ShellViewModel viewModel)
        {
            _buttonViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true; // Button is enable when true
        }

        public void Execute(object parameter)
        {
            _buttonViewModel.OnHomeButtonClick();
        }
    }
}
