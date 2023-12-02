using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingApp.Data.Models
{
	public class AccountsDTO
    {
        public string? Uid { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public required ICollection<OrderHeaderDTO> OrderHeaders { get; set; }
    }
}

