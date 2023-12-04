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
    public class PayCartWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public PayCartWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Execute(string accountID)
        {
            ICart searchedCart = await CartsRepository.GetCart(accountID);
            CartService service = new(_dbContext);

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
                whenValidatedCart: @event =>
                {
                    result = 403;
                    return @event;
                },
                whenCalculatedCart: calculatedCart =>
                {
                    ICart cart = service.PayCart(calculatedCart).Result;
                    if (cart is PaidCart)
                    {
                        if (CartsRepository.ChangeCartState(accountID, cart).Result)
                        {
                            result = 200;
                        }
                    }
                    return calculatedCart;
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
