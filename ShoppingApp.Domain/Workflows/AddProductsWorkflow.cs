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
                whenValidatedCart: validatedCart =>
                {
                    Task task = Task.Run(async () =>
                    {
                        
                        PendingCart pendingCart = new();
                        var productsToAdd = validatedCart.products.SelectMany(product =>
                        {
                            return Enumerable.Repeat(product.Uid, product.Quantity);
                        });

                        pendingCart.products.AddRange(productsToAdd);
                        await service.AddProductToCart(pendingCart, productCode);
                        if (await CartsRepository.ChangeCartState(accountID, pendingCart))
                        {
                            response.Success = true;
                            response.StatusCode = 201;
                        }
                    });
                    task.Wait();

                    return validatedCart;
                },
                whenCalculatedCart: calculatedCart =>
                {
                    Task task = Task.Run(async () =>
                    {

                        PendingCart pendingCart = new();
                        var productsToAdd = calculatedCart.products.SelectMany(product =>
                        {
                            return Enumerable.Repeat(product.Uid, product.Quantity);
                        });

                        pendingCart.products.AddRange(productsToAdd);
                        await service.AddProductToCart(pendingCart, productCode);
                        if (await CartsRepository.ChangeCartState(accountID, pendingCart))
                        {
                            response.Success = true;
                            response.StatusCode = 201;
                        }
                    });
                    task.Wait();

                    return calculatedCart;
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
