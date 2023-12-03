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

        public async Task<bool> Execute(string accountID)
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
                    ICart cart = service.ValidateCart(pendingCart).Result;
                    if (cart is ValidatedCart)
                    {
                        succeded = CartsRepository.ChangeCartState(accountID, cart).Result;
                    }

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

