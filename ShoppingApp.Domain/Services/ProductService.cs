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
using ShoppingApp.Common.Models;
using ShoppingApp.Data;

namespace ShoppingApp.Domain.Services
{
    public class ProductService
    {
        private readonly IDbContextFactory dbContextFactory;
        ProductRepository productRepository;
        public ProductService(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
            productRepository = new(dbContextFactory.CreateDbContext());
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

        public async Task<int> GetQuantity(string productCode)
        {
            return await productRepository.GetQuantity(productCode);
        }

        public async Task<List<Product>> GetAllProdcuts()
        {
            List<ProductDTO> productsDTO = await productRepository.GetAllProducts();
            List<Product> products = new();

            productsDTO.ForEach(p => products.Add(ProductMapper.MapToProduct(p)));
            return products;
        }

        public async Task<Product> GetProductByUid(string productCode)
        {
            ProductDTO productDTO = await productRepository.GetProductByUid(productCode);
            return ProductMapper.MapToProduct(productDTO);
        }
    }
}
