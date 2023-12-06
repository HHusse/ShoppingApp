using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApp.Data.Models
{
    public class AccountDTO
    {
        public string Uid { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Column(TypeName = "TEXT")]
        public string Address { get; set; }
        [Column(TypeName = "TEXT")]
        public string Password { get; set; }

        public required ICollection<OrderHeaderDTO> OrderHeaders { get; set; }
    }
}

