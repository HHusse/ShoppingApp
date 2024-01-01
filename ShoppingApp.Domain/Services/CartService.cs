using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ShoppingApp.Data.Repositories;
using ShoppingApp.Domain.Models;
using static ShoppingApp.Domain.Models.Cart;
using ShoppingApp.Common.Models;
using ShoppingApp.Data;

namespace ShoppingApp.Domain.Services
{
    internal class CartService
    {
        private readonly IDbContextFactory dbContextFactory;
        ProductService productService;

        public CartService(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
            productService = new(dbContextFactory);
        }

        public async Task<bool> AddProductToCart(PendingCart cart, string productCode)
        {
            int count = cart.products.Count();
            cart.products.Add(productCode);
            return (count + 1) == cart.products.Count();
        }

        public async Task<bool> RemoveProductFromCart(PendingCart cart, string productCode)
        {
            return cart.products.Remove(productCode);
        }

        public async Task<ICart> ValidateCart(PendingCart pendingCart)
        {
            if (pendingCart.products.Count == 0)
            {
                return pendingCart;
            }

            List<Product> products = new List<Product>();
            foreach (var product in pendingCart.products)
            {
                if (products.Exists(p => p.Uid == product))
                {
                    products.First(p => p.Uid == product).Quantity++;
                    continue;
                }

                var validatedProduct = await productService.ValidateProduct(product);

                Product newProduct = validatedProduct.IfNone(() => new Product());
                if (String.IsNullOrEmpty(newProduct.Uid))
                {
                    return pendingCart;
                }

                newProduct.Quantity = 1;
                products.Add(newProduct);

            }
            foreach (var product in products)
            {
                bool isEnough = await productService.VerifyQunatity(product.Uid, product.Quantity);
                if (!isEnough)
                {
                    return pendingCart;
                }
                product.Price = product.Price * product.Quantity;
            }

            ValidatedCart validatedCart = new(products);
            return validatedCart;

        }

        public async Task<ICart> CalculateCart(ValidatedCart validatedCart)
        {
            double sum = validatedCart.products.Sum(product => product.Price);
            CalculatedCart calculatedCart = new(validatedCart.products, sum);
            return calculatedCart;
        }

        public async Task<ICart> PayCart(CalculatedCart calculatedCart)
        {
            DateTime date = DateTime.Now;
            PaidCart paidCart = new(calculatedCart.products, calculatedCart.price, date);
            return paidCart;
        }

        public async Task<PendingCart> CleanUpCart(PendingCart pendingCart)
        {
            if (pendingCart.products.Count == 0)
            {
                return pendingCart;
            }

            Dictionary<string, int> products = new();
            List<string> productsToRemove = new();
            foreach (var product in pendingCart.products)
            {
                if (products.Keys.Exists(p => p == product))
                {
                    products[product] += 1;
                    continue;
                }

                var validatedProduct = await productService.ValidateProduct(product);

                Product validProduct = validatedProduct.IfNone(() => new Product());
                if (String.IsNullOrEmpty(validProduct.Uid))
                {
                    productsToRemove.Add(product);
                    continue;
                }

                validProduct.Quantity = 1;
                products.Add(validProduct.Uid, validProduct.Quantity);
            }

            foreach (var productToRemove in productsToRemove)
            {
                pendingCart.products.Remove(productToRemove);
            }

            foreach (var product in products)
            {
                int quantity = await productService.GetQuantity(product.Key);
                if (quantity == 0)
                {
                    pendingCart.products.RemoveAll(p => p == product.Key);
                    continue;
                }

                if (quantity < product.Value)
                {
                    int quantityToRemove = product.Value - quantity;

                    var productToRemove = pendingCart.products.FirstOrDefault(p => p == product.Key);

                    if (productToRemove != null)
                    {
                        for (int count = 0; count < quantityToRemove; count++)
                        {
                            pendingCart.products.Remove(productToRemove);
                        }
                    }
                }
            }

            return pendingCart;
        }
    }
}
