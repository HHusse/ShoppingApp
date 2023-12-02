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
            ICart searchedCart = await CartsRepository.GetCart(accountID);

            bool isPendingCart = false;
            searchedCart.Match(
                whenEmptyCart: @event =>
                {
                    isPendingCart = false;
                    return @event;
                },
                whenPendingCart: @event =>
                {
                    isPendingCart = true;
                    return @event;
                },
                whenValidatedCart: @event =>
                {
                    isPendingCart = false;
                    return @event;
                },
                whenCalculatedCart: @event =>
                {
                    isPendingCart = false;
                    return @event;
                },
                whenPaidCart: @event =>
                {
                    isPendingCart = false;
                    return @event;
                }
            );

            if (!isPendingCart)
            {
                return false;
            }

            CartService service = new(_dbContext);
            ICart cart = await service.ValidateCart((PendingCart)searchedCart);
            bool succeded = false;
            cart.Match(
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
                    whenValidatedCart: @event =>
                    {
                        succeded = true;
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

            if (succeded)
            {
                await CartsRepository.ChangeCartState(accountID, cart);
            }

            return succeded;
        }

    }
}

