using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ShoppingApp.Domain.Models;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain
{
	public static class PendingCartsRepository
	{
		public static ConcurrentDictionary<string, PendingCart> pendingCarts = new();

		public static async Task<bool> CreateNewCart(string accountID)
		{
			PendingCart newCart = new();
			return pendingCarts.TryAdd(accountID, newCart);
		}

		public static async Task<bool> AddNewProduct(string accountID, string product)
		{
			await CreateNewCart(accountID);
            PendingCart oldCart;
			pendingCarts.TryGetValue(accountID, out oldCart);
            PendingCart updatedCart = oldCart;
			updatedCart.products.Add(product);
			return pendingCarts.TryUpdate(accountID, updatedCart, oldCart);
        }

		public static async Task<bool> RemoveCart(string accountID)
		{
			return pendingCarts.Remove(accountID, out _);
		}

		public static async Task<PendingCart> GetCart(string accountID)
		{
            await CreateNewCart(accountID);
            PendingCart cart;
            pendingCarts.TryGetValue(accountID, out cart);
			return cart;
        }
	}
}

