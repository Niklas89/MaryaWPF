using AutoMapper;
using Caliburn.Micro;
using MaryaWPF.Library.Api;
using MaryaWPF.Library.Models;
using MaryaWPF.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.ViewModels
{
    public class PartnerDetailsViewModel: Screen
    {
        IPartnerEndpoint _partnerEndpoint;
        IMapper _mapper;
        private UserPartnerDisplayModel _selectedPartner;
        public UserPartnerDisplayModel SelectedPartner
        {
            get { return _selectedPartner; }
            set
            {
                _selectedPartner = value;
                NotifyOfPropertyChange(() => SelectedPartner);
            }
        }


        private BindingList<UserPartnerDisplayModel> _partners;

        public BindingList<UserPartnerDisplayModel> Partners
        {
            get { return _partners; }
            set
            {
                _partners = value;
                NotifyOfPropertyChange(() => Partners);
            }
        }

        public PartnerDetailsViewModel(IPartnerEndpoint partnerEndpoint, IMapper mapper)
        {
            _partnerEndpoint = partnerEndpoint;
            _mapper = mapper;
        }

        private string _selectedFirstName;

        public string SelectedFirstName
        {
            get { return _selectedFirstName; }
            set { 
                _selectedFirstName = value;
                NotifyOfPropertyChange(() => SelectedFirstName);
            }
        }

        private string _selectedLastName;

        public string SelectedLastName
        {
            get { return _selectedLastName; }
            set
            {
                _selectedLastName = value;
                NotifyOfPropertyChange(() => SelectedLastName);
            }
        }


        public void UpdatePartnerDetails(UserPartnerDisplayModel selectedPartner)
        {
            _selectedPartner = selectedPartner;
            List<UserPartnerDisplayModel> partnerList = new List<UserPartnerDisplayModel>
            {
                selectedPartner
            };
            Partners = new BindingList<UserPartnerDisplayModel>(partnerList);
            SelectedFirstName = selectedPartner.FirstName;
            SelectedLastName = selectedPartner.LastName;

        }

        public async Task Edit()
        {
            UserPartnerModel partner = _mapper.Map<UserPartnerModel>(SelectedPartner);
            partner.FirstName = SelectedFirstName;
            partner.LastName = SelectedLastName;
            await _partnerEndpoint.UpdatePartner(partner);
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
