using System;
using Data;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class AddProductsWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public AddProductsWorkflow(ShoppingAppDbContext dbContext)
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
                    Task task = Task.Run(async () =>
                    {
                        await CartsRepository.ChangeCartState(accountID, new PendingCart());
                        if (await CartsRepository.AddNewProduct(accountID, productCode))
                        {
                            response.Success = true;
                            response.StatusCode = 201;
                        }
                    });
                    task.Wait();

                    return @event;
                },
                whenPendingCart: pendingCart =>
                {
                    Task task = Task.Run(async () =>
                    {
                        await service.AddProductToCart(pendingCart, productCode);
                        if (await CartsRepository.ChangeCartState(accountID, pendingCart))
                        {
                            response.Success = true;
                            response.StatusCode = 201;
                        }
                    });
                    task.Wait();

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
