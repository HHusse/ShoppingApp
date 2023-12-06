using System;
using Data;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class RemoveProductWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public RemoveProductWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GeneralWorkflowResponse> Execute(string accountID, string productCode)
        {
            CartService service = new(_dbContext);
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
                    response.Success = true;
                    response.StatusCode = 200;
                    Task.Run(async () =>
                    {
                        await service.RemoveProductFromCart(pendingCart, productCode);
                        await CartsRepository.ChangeCartState(accountID, pendingCart);
                    });

                    return pendingCart;
                },
                whenValidatedCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is an validated cart";
                    response.StatusCode = 403;
                    return @event;
                },
                whenCalculatedCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is an calculated cart";
                    response.StatusCode = 403;
                    return @event;
                },
                whenPaidCart: @event =>
                {
                    response.Success = false;
                    response.Message = "Is an paid cart";
                    response.StatusCode = 403;
                    return @event;
                }
            );

            return response;
        }
    }
}

