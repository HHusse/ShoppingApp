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

        public async Task<int> Execute(string accountID)
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
                whenPendingCart: @event =>
                {
                    result = 403;
                    return @event;
                },
                whenValidatedCart: validatedCart =>
                {
                    CalculatedCart newCart = (CalculatedCart)service.CalculateCart(validatedCart).Result;
                    if (CartsRepository.ChangeCartState(accountID, newCart).Result)
                    {
                        result = 200;
                    }
                    return validatedCart;
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

