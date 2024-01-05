using System;
using Data;
using LanguageExt;
using ShoppingApp.Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class ValidateCartWorkflow
    {
        private readonly IDbContextFactory dbContextFactory;

        public ValidateCartWorkflow(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<GeneralWorkflowResponse> Execute(string accountID)
        {
            CartService service = new(dbContextFactory);
            ICart searchedCart = await CartsRepository.GetCart(accountID);

            GeneralWorkflowResponse response = new();
            searchedCart.Match(
                whenEmptyCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is an empty cart";
                    response.StatusCode = 403;
                    return @event;
                },
                whenPendingCart: pendingCart =>
                {
                    ICart cart = service.ValidateCart(pendingCart).Result;
                    if (cart is ValidatedCart)
                    {
                        if (CartsRepository.ChangeCartState(accountID, cart).Result)
                        {
                            response.Success = true;
                            response.StatusCode = 200;
                        }
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = "Invalid product in cart";
                        response.StatusCode = 400;
                    }

                    return pendingCart;
                },
                whenValidatedCart: @event =>
                {
                    response.Success = true;
                    response.StatusCode = 200;
                    return @event;
                },
                whenCalculatedCart: calculatedCart =>
                {
                    Task task = Task.Run(async () =>
                    {

                        ValidatedCart cart = new(calculatedCart.products);
                        if (await CartsRepository.ChangeCartState(accountID, cart))
                        {
                            response.Success = true;
                            response.StatusCode = 200;
                        }
                    });
                    task.Wait();

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

