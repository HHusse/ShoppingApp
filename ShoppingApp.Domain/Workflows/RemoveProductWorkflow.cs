using System;
using Data;
using ShoppingApp.Data;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class RemoveProductWorkflow
    {
        private readonly IDbContextFactory dbContextFactory;

        public RemoveProductWorkflow(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<GeneralWorkflowResponse> Execute(string accountID, string productCode)
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
                    Task task = Task.Run(async () =>
                    {
                        if (!(await service.RemoveProductFromCart(pendingCart, productCode)))
                        {
                            response.Success = false;
                            response.Message = "The product could not be deleted because it does not exist in the cart";
                            response.StatusCode = 400;
                            return;
                        }
                        if (await CartsRepository.ChangeCartState(accountID, pendingCart))
                        {
                            response.Success = true;
                            response.StatusCode = 200;
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
                        if (!(await service.RemoveProductFromCart(pendingCart, productCode)))
                        {
                            response.Success = false;
                            response.Message = "The product could not be deleted because it does not exist in the cart";
                            response.StatusCode = 400;
                            return;
                        }
                        if (await CartsRepository.ChangeCartState(accountID, pendingCart))
                        {
                            response.Success = true;
                            response.StatusCode = 200;
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
                        if (!(await service.RemoveProductFromCart(pendingCart, productCode)))
                        {
                            response.Success = false;
                            response.Message = "The product could not be deleted because it does not exist in the cart";
                            response.StatusCode = 400;
                            return;
                        }
                        if (await CartsRepository.ChangeCartState(accountID, pendingCart))
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

