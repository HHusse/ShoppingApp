using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using ShoppingApp.Data.Repositories;
using ShoppingApp.Domain.Models;
using LanguageExt;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Mappers;


namespace ShoppingApp.Domain.Services
{
    public class ProductService
    {
        private readonly ShoppingAppDbContext _dbContext;
        ProductRepository productRepository;
        public ProductService(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            productRepository = new(_dbContext);
        }
        public async Task<Option<Product>> ValidateProduct(string productCode)
        {
            ProductDTO product = await productRepository.SearchProduct(productCode);
            Product validatedProduct = ProductMapper.MapToProduct(product);
            return validatedProduct == null ? Option<Product>.None : Option<Product>.Some(validatedProduct);

        }
        public async Task<bool> VerifyQunatity(string productCode, int quantity)
        {
            return await productRepository.VerifyQunatity(productCode, quantity);
        }
    }
}
