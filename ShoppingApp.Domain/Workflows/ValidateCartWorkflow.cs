using System;
using Data;
using LanguageExt;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class ValidateCartWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public ValidateCartWorkflow(ShoppingAppDbContext dbContext)
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
                whenPendingCart: pendingCart =>
                {
                    ICart cart = service.ValidateCart(pendingCart).Result;
                    if (cart is ValidatedCart)
                    {
                        if (CartsRepository.ChangeCartState(accountID, cart).Result)
                        {
                            result = 200;
                        }
                    }
                    else
                    {
                        result = 400;
                    }

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

