using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using ShoppingApp.Data.Models;

namespace ShoppingApp.Data.Models
{
    public class ProductDTO
    {
        public string Uid { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "TEXT")]
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public ICollection<OrderLineDTO> OrderLines { get; set; }

    }
}

