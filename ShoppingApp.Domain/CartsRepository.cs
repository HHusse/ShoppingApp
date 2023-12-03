using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Identity.Client;
using ShoppingApp.Domain.Models;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain
{
    public static class CartsRepository
    {
        public static ConcurrentDictionary<string, ICart> carts = new();

        public static async Task<bool> CreateNewCart(string accountID)
        {
            EmptyCart newCart = new();
            return carts.TryAdd(accountID, newCart);
        }

        public static async Task<bool> AddNewProduct(string accountID, string product)
        {
            ICart oldCart;
            carts.TryGetValue(accountID, out oldCart);
            ICart updatedCart = oldCart;
            updatedCart.Match(
                    whenEmptyCart: @event =>
                    {
                        return @event;
                    },
                    whenPendingCart: pendingCart =>
                    {
                        pendingCart.products.Add(product);
                        return pendingCart;
                    },
                    whenValidatedCart: @event =>
                    {
                        return @event;
                    },
                    whenCalculatedCart: @event =>
                    {
                        return @event;
                    },
                    whenPaidCart: @event =>
                    {
                        return @event;
                    }
                );

            return carts.TryUpdate(accountID, updatedCart, oldCart);
        }

        public static async Task<bool> RemoveCart(string accountID)
        {
            return carts.Remove(accountID, out _);
        }

        public static async Task<ICart> GetCart(string accountID)
        {
            await CreateNewCart(accountID);
            ICart cart;
            carts.TryGetValue(accountID, out cart);
            return cart;
        }

        public static async Task<bool> ChangeCartState(string accountID, ICart newCartState)
        {
            ICart oldCart;
            carts.TryGetValue(accountID, out oldCart);
            return carts.TryUpdate(accountID, newCartState, oldCart);
        }
    }
}

