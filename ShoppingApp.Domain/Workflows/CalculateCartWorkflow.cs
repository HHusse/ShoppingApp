﻿using System;
using Data;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class CalculateCartWorkflow
    {
        private readonly IDbContextFactory dbContextFactory;

        public CalculateCartWorkflow(IDbContextFactory dbContextFactory)
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
                whenPendingCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is a pending cart";
                    response.StatusCode = 403;
                    return @event;
                },
                whenValidatedCart: validatedCart =>
                {
                    CalculatedCart newCart = (CalculatedCart)service.CalculateCart(validatedCart).Result;
                    if (CartsRepository.ChangeCartState(accountID, newCart).Result)
                    {
                        response.Success = true;
                        response.StatusCode = 200;
                    }
                    return validatedCart;
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

