using System;
namespace ShoppingApp.DataAccess.Models
{
	public class OrderLineDTO
	{
        public string? Uid { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public OrderHeaderDTO? OrderHeader { get; set; }
        public string? OrderHeaderUid { get; set; }

        public ProductDTO? Product { get; set; }
        public string? ProductUid { get; set; }
    }
}

