using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MaryaWPF.ViewModels
{
    public class PartnerDisplayViewModel : Screen
    {
        private readonly StatusInfoViewModel _status;
        private readonly IWindowManager _window;
        private readonly IPartnerEndpoint _partnerEndpoint;
        private readonly IServiceEndpoint _serviceEndpoint;
        BindingList<UserPartnerModel> _partners;

        public BindingList<UserPartnerModel> Partners
        {
            get
            {
                return _partners;
            }
            set
            {
                _partners = value;
                NotifyOfPropertyChange(() => Partners);
            }
        }

        public PartnerDisplayViewModel(StatusInfoViewModel status, IWindowManager window, IPartnerEndpoint partnerEndpoint, IServiceEndpoint serviceEndpoint)
        {
            _status = status;
            _window = window;
            _partnerEndpoint = partnerEndpoint;
            _serviceEndpoint = serviceEndpoint;
        }

        // Wait before the View loads
        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            try
            {
                await LoadPartners();
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

        private async Task LoadPartners()
        {
            var partnerList = await _partnerEndpoint.GetAll();
            var categoriesList = await _serviceEndpoint.GetAll();

            foreach(var partner in partnerList)
            {
                foreach(var category in categoriesList)
                {
                    if(partner.Partner.IdCategory == category.Id)
                    {
                        partner.Partner.CategoryName = category.Name;
                    }
                }
            }
            Partners = new BindingList<UserPartnerModel>(partnerList);
        }
    }
}
