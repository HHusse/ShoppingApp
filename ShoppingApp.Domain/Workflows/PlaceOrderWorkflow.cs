using Azure;
using Data;
using ShoppingApp.Data;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using ShoppingApp.Events;
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
        private readonly IDbContextFactory dbContextFactory;
        IEventSender sender;

        public PlaceOrderWorkflow(IDbContextFactory dbContextFactory, IEventSender sender)
        {
            this.dbContextFactory = dbContextFactory;
            this.sender = sender;
        }

        public async Task<GeneralWorkflowResponse> Execute(string accountID)
        {
            ICart searchedCart = await CartsRepository.GetCart(accountID);
            OrderService service = new(dbContextFactory, sender);

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
                        await CartsRepository.RemoveCart(accountID);
                    });
                    response.Success = true;
                    response.StatusCode = 202;
                    return paidCart;
                }
            );

            return response;

        }

    }
}
