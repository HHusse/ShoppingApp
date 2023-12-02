using ShoppingApp.Data.Models;

namespace ShoppingApp.Data.Models
{
	public class ProductDTO
	{
        public string? Uid { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }

        public required ICollection<OrderLineDTO> OrderLines { get; set; }

    }
}

