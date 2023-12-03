using System;
using Data;
using ShoppingApp.Domain.Models;
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
                    Task.Run(async () =>
                    {
                        double sum = validatedCart.products.Sum(product => product.Price);
                        CalculatedCart calculatedCart = new(validatedCart.products, sum);
                        succeded = await CartsRepository.ChangeCartState(accountID, calculatedCart);
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

