using System;
using Data;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class RemoveProductWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public RemoveProductWorkflow(ShoppingAppDbContext dbContext)
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
                    result = 403;
                    return @event;
                },
                whenPendingCart: pendingCart =>
                {
                    result = 200;
                    Task.Run(async () =>
                    {
                        await service.RemoveProductFromCart(pendingCart, productCode);
                        await CartsRepository.ChangeCartState(accountID, pendingCart);
                    });

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

