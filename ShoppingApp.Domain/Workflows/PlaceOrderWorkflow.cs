using Azure;
using Data;
using ShoppingApp.Domain.ResponseModels;
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

        public async Task<GeneralWorkflowResponse> Execute(string accountID)
        {
            ICart searchedCart = await CartsRepository.GetCart(accountID);
            OrderService service = new(_dbContext);

            GeneralWorkflowResponse response = new();
            searchedCart.Match(
                whenEmptyCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is an empty cart";
                    response.StatusCode = 403;
                    return @event;
                },
                whenPendingCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is a pending cart";
                    response.StatusCode = 403;
                    return @event;
                },
                whenValidatedCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is a validated cart";
                    response.StatusCode = 403;
                    return @event;
                },
                whenCalculatedCart: @event =>
                {
                    response.Success = false;
                    response.Message = "The cart must be paid";
                    response.StatusCode = 402;
                    return @event;
                },
                whenPaidCart: paidCart =>
                {
                    Task task = Task.Run(async () =>
                    {
                        await service.PlaceOrder(accountID, paidCart);
                        if (await CartsRepository.RemoveCart(accountID))
                        {
                            response.Success = true;
                            response.StatusCode = 200;
                        }
                    });
                    task.Wait();

                    return paidCart;
                }
            );

            return response;

        }

    }
}
