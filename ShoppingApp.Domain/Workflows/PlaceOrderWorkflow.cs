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

        public async Task<int> Execute(string accountID)
        {
            ICart searchedCart = await CartsRepository.GetCart(accountID);
            OrderService service = new(_dbContext);

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
                whenCalculatedCart: @event =>
                {
                    result = 402;
                    return @event;
                },
                whenPaidCart: paidCart =>
                {
                    Task task = Task.Run(async () =>
                    {
                        await service.PlaceOrder(accountID, paidCart);
                        if (await CartsRepository.RemoveCart(accountID))
                        {
                            result = 200;
                        }
                    });
                    task.Wait();
                    return paidCart;
                }
            );

            return result;

        }

    }
}
