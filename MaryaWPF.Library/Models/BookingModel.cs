using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int? NbHours { get; set; }
        public string Description { get; set; } 
        public bool Accepted { get; set; }
        public float TotalPrice { get; set; }
        public DateTime? CancelDate { get; set; }
        public bool IsCancelled { get; set; }
        public bool ServiceDone { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreatedAt { get; set; }
        public int IdClient { get; set; }
        public int? IdPartner { get; set; }
        public int IdService { get; set; }
    }
}
