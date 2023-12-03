using System;
using Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class CalculateCartWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public CalculateCartWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Execute(string accountID, string product)
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
                whenPendingCart: @event =>
                {
                    succeded = false;
                    return @event;
                },
                whenValidatedCart: validatedCart =>
                {
                    succeded = true;
                    Task.Run(async () =>
                    {
                        CalculatedCart newCart = (CalculatedCart)await service.CalculateCart(validatedCart);
                        succeded = await CartsRepository.ChangeCartState(accountID, newCart);
                    });
                    return validatedCart;
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

