using System;
namespace ShoppingApp.DataAccess.Models
{
	public class OrderHeaderDTO
	{
        public string? Uid { get; set; }
        public string? AccountId { get; set; }
        public string? Date { get; set; }
        public double Total { get; set; }
    }
}

