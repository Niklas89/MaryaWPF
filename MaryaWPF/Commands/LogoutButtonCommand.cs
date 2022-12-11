using MaryaWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MaryaWPF.Commands
{
    public class LogoutButtonCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        ShellViewModel _buttonViewModel;

        public LogoutButtonCommand(ShellViewModel viewModel)
        {
            _buttonViewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true; // Button is enable when true
        }

        public async void Execute(object parameter)
        {
            await _buttonViewModel.LogOut();
        }
    }
}
