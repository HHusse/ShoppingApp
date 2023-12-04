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
        public async Task<int> Execute(string accountID, string productCode)
        {
            CartService service = new(_dbContext);

            ICart searchedCart = await CartsRepository.GetCart(accountID);

            int result = 500;
            searchedCart.Match(
                whenEmptyCart: @event =>
                {
                    Task task = Task.Run(async () =>
                    {
                        await CartsRepository.ChangeCartState(accountID, new PendingCart());
                        if (await CartsRepository.AddNewProduct(accountID, productCode))
                        {
                            result = 201;
                        }
                    });
                    task.Wait();

                    return @event;
                },
                whenPendingCart: pendingCart =>
                {
                    Task task = Task.Run(async () =>
                    {
                        await service.AddProductToCart(pendingCart, productCode);
                        if (await CartsRepository.ChangeCartState(accountID, pendingCart))
                        {
                            result = 201;
                        }
                    });
                    task.Wait();

                    return pendingCart;
                },
                whenValidatedCart: @event =>
                {
                    result = 403;
                    return @event;
                },
                whenCalculatedCart: @event =>
                {
                    result = 403;
                    return @event;
                },
                whenPaidCart: @event =>
                {
                    result = 403;
                    return @event;
                }
            );

            return result;
        }
    }
}
