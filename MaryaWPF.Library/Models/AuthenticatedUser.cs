using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaryaWPF.Library.Models
{
    public class AuthenticatedUser
    {
        public string AccessToken { get; set; }
        public string Email { get; set; }
    }
}
