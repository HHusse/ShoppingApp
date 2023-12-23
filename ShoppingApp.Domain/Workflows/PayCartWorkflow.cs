using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
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

        public async Task<GeneralWorkflowResponse> Execute(string accountID)
        {
            ICart searchedCart = await CartsRepository.GetCart(accountID);
            CartService service = new(_dbContext);

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
                whenCalculatedCart: calculatedCart =>
                {
                    ICart cart = service.PayCart(calculatedCart).Result;
                    if (cart is PaidCart)
                    {
                        if (CartsRepository.ChangeCartState(accountID, cart).Result)
                        {
                            response.Success = true;
                            response.StatusCode = 200;
                        }
                    }
                    return calculatedCart;
                },
                whenPaidCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is a paid cart";
                    response.StatusCode = 403;
                    return @event;
                }
            );

            return response;

        }


    }
}
