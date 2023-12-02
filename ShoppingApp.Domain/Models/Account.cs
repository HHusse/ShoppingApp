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

        public Account(string? lastName, string? firstName, string? email, string? password)
        {
            Uid = Guid.NewGuid();
            this.LastName = lastName;
            this.FirstName = firstName;
            this.Email = email;
            this.Password = password;
        }

        public Guid Uid { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}

