﻿using System;
using Data;
using ShoppingApp.Data;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class CleanUpCartWorkflow
    {
        private readonly IDbContextFactory dbContextFactory;

        public CleanUpCartWorkflow(IDbContextFactory dbContextFactory)
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
                    ICart cart = service.CleanUpCart(pendingCart).Result;
                    if (CartsRepository.ChangeCartState(accountID, cart).Result)
                    {
                        response.Success = true;
                        response.StatusCode = 200;
                    }

                    return pendingCart;
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
                    response.Message = "Is a calculated cart";
                    response.StatusCode = 403;
                    return @event;
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

