using System;
using Data;
using ShoppingApp.Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Workflows
{
    public class GetCartWorkflow
    {
        private readonly IDbContextFactory dbContextFactory;

        public GetCartWorkflow(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        public async Task<CartResponse> Execute(string accountID)
        {
            CartService service = new(dbContextFactory);

            ICart searchedCart = await CartsRepository.GetCart(accountID);

            CartResponse res = new CartResponse();
            searchedCart.Match(
                    whenEmptyCart: @event =>
                    {
                        res.State = "empty";
                        res.Cart = new { };
                        return @event;
                    },
                    whenPendingCart: pendingCart =>
                    {
                        res.State = "pending";
                        var groupedProducts = pendingCart.products
                            .GroupBy(p => p)
                            .Select(group => new { id = group.Key, quantity = group.Count() });

                        res.Cart = new { products = groupedProducts };

                        return pendingCart;
                    },
                    whenValidatedCart: validatedCart =>
                    {
                        res.State = "validated";
                        res.Cart = new { validatedCart.products };

                        return validatedCart;
                    },
                    whenCalculatedCart: calculatedCart =>
                    {
                        res.State = "calculated";
                        res.Cart = new { calculatedCart.products, calculatedCart.price };

                        return calculatedCart;
                    },
                    whenPaidCart: paidCart =>
                    {
                        res.State = "paid";
                        res.Cart = new { paidCart.products, paidCart.finalPrice, paidCart.data };

                        return paidCart;
                    }
                );

            return res;
        }
    }
}

