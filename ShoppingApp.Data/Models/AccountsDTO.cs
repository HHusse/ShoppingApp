using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Data.Models
{
	public class AccountsDTO
    {
        [Key]
        string? Uid { get; set; }
        string? LastName { get; set; }
        string? FirstName { get; set; }
        string? Email { get; set; }
        string? Password { get; set; }
    }
}

