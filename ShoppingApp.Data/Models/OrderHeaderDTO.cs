using System;
namespace ShoppingApp.Data.Models
{
	public class OrderHeaderDTO
	{
        public string? Uid { get; set; }
        public string? AccountId { get; set; }
        public string? Date { get; set; }
        public double Total { get; set; }

        public required ICollection<OrderLineDTO> OrderLines { get; set; }
    }
}

