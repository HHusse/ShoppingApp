using System;
namespace ShoppingApp.Domain.Models
{
    public class Account
    {
        public Account(Guid uid, string? lastName, string? firstName, string? email, string? password)
        {
            Uid = uid;
            this.LastName = lastName;
            this.FirstName = firstName;
            this.Email = email;
            this.Password = password;
        }

        Guid Uid { get; set; }
        string? LastName { get; set; }
        string? FirstName { get; set; }
        string? Email { get; set; }
        string? Password { get; set; }
    }
}

