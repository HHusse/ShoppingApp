using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    internal class PayCartWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public PayCartWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Execute(string accountID)
        {
            ICart searchedCart = await CartsRepository.GetCart(accountID);
            CartService service = new(_dbContext);

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
                whenValidatedCart: @event =>
                {
                    succeded = false;
                    return @event;
                },
                whenCalculatedCart: calculatedCart =>
                {
                    ICart cart = service.PayCart(calculatedCart).Result;
                    if (cart is PaidCart)
                    {
                        succeded = CartsRepository.ChangeCartState(accountID, cart).Result;
                    }
                    return calculatedCart;
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
