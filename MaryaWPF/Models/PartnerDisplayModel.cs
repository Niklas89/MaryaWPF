using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Models
{
    public class PartnerDisplayModel
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Img { get; set; }
        public string SIRET { get; set; }
        public string IBAN { get; set; }
        public int IdCategory { get; set; }
        public string CategoryName { get; set; }
    }
}
