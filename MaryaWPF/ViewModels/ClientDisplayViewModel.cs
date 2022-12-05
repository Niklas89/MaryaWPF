using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaryaWPF.ViewModels;

public class ClientDisplayViewModel : Screen
{
    private readonly StatusInfoViewModel _status;
    private readonly IWindowManager _window;
    private readonly IClientEndpoint _clientEndpoint;
    BindingList<UserClientModel> _clients;

    public BindingList<UserClientModel> Clients
    {
        get
        {
            return _clients;
        }
        set
        {
            _clients = value;
            NotifyOfPropertyChange(() => Clients);
        }
    }

    public ClientDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IClientEndpoint clientEndpoint)
    {
        _status = status;
        _window = window;
        _clientEndpoint = clientEndpoint;
    }

    // Wait before the View loads
    protected override async void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        try
        {
            await LoadClients();
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

    private async Task LoadClients()
    {
        var clientList = await _clientEndpoint.GetAll();
        Clients = new BindingList<UserClientModel>(clientList);
    }

}
