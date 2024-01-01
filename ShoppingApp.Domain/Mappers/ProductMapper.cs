using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingApp.Common.Models;

namespace ShoppingApp.Domain.Mappers
{
    internal class ProductMapper
    {
        public static Product MapToProduct(ProductDTO productDTO)
        {
            if (productDTO == null)
                return null;

            return new Product(
                productDTO.Uid,
                productDTO.Name,
                productDTO.Description,
                productDTO.Quantity,
                productDTO.Price
            );
        }

        public static ProductDTO MapToAccountDTO(Product product)
        {
            if (product == null)
                return null;

            return new ProductDTO
            {
                Uid = product.Uid,
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Price = product.Price,

                OrderLines = new List<OrderLineDTO>()
            };
        }
    }
}
