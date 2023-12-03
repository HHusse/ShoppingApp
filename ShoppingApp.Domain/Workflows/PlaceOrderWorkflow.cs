using Data;
using ShoppingApp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class PlaceOrderWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public PlaceOrderWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Execute(string accountID)
        {
            ICart searchedCart = await CartsRepository.GetCart(accountID);
            OrderService service = new(_dbContext);

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
                whenCalculatedCart: @event =>
                {
                    succeded = false;
                    return @event;
                },
                whenPaidCart: paidCart =>
                {
                    service.PlaceOrder(accountID, paidCart);
                    CartsRepository.RemoveCart(accountID);
                    succeded = true;
                    return paidCart;
                }
            );

            return succeded;

        }

    }
}
