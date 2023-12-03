using System;
using Data;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class AddProductsWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public AddProductsWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> Execute(string accountID, string productCode)
        {
            CartService service = new(_dbContext);
            return await service.AddProductToCart(accountID, productCode);
        }
    }
}
