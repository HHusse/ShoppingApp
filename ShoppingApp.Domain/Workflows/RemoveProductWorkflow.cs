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

        public async Task<bool> Execute(string accountID, string productCode)
        {
            CartService service = new(_dbContext);
            ICart searchedCart = await CartsRepository.GetCart(accountID);

            bool succeded = false;
            searchedCart.Match(
                whenEmptyCart: @event =>
                {
                    succeded = false;
                    return @event;
                },
                whenPendingCart: pendingCart =>
                {
                    succeded = true;
                    Task.Run(async () =>
                    {
                        await service.RemoveProductFromCart(pendingCart, productCode);
                        await CartsRepository.ChangeCartState(accountID, pendingCart);
                    });

                    return pendingCart;
                },
                whenValidatedCart: @event =>
                {
                    succeded = false;
                    return @event;
                },
                whenCalculatedCart: @event =>
                {
                    succeded = false;
                    return @event;
                },
                whenPaidCart: @event =>
                {
                    succeded = false;
                    return @event;
                }
            );

            return succeded;
        }
    }
}

