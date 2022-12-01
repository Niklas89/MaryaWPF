using System;

namespace MaryaWPF.Library.Models
{
    public interface ILoggedInUserModel
    {
        DateTime CreatedAt { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string Id { get; set; }
        string LastName { get; set; }
        string Token { get; set; }

        void ResetUserModel();
    }
}