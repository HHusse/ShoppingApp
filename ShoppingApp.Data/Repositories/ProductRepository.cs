using Data;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApp.Data.Repositories
{
    public class ProductRepository
    {
        private readonly ShoppingAppDbContext _dbContext;

        public ProductRepository(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<ProductDTO> SearchProduct(string? productCode)
        {
            ProductDTO productDTO = await _dbContext.Products.FirstOrDefaultAsync(product => product.Uid == productCode);
            return productDTO;
        }
    }
}
