using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace MaryaWPF.ViewModels
{
    public class ShellViewModel : Conductor<object>
    {
        // Test code for Dependency Injection with Bootstrapper
        /*
        private ICalculations _calculations;

        public ShellViewModel(ICalculations calculations)
        {
            _calculations = calculations;
        } */

        private LoginViewModel _loginVM;

        public ShellViewModel(LoginViewModel loginVM)
        {
            _loginVM = loginVM;
            ActivateItemAsync(_loginVM); // Activate LoginViewModel on ShellViewModel so I can use it later
        }

    }
}
