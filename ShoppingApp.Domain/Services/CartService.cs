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

namespace ShoppingApp.Domain.Services
{
    internal class CartService
    {
        private readonly ShoppingAppDbContext _dbContext;
        ProductService productService;

        public CartService(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            productService = new(_dbContext);
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
            List<Product> products = new List<Product>();
            foreach (var product in pendingCart.products)
            {
                if (products.Exists(p => p.Uid == product))
                {
                    products.First(p => p.Uid == product).Quantity++;
                    continue;
                }

                var validatedProduct = await productService.ValidateProduct(product);

                Product newProduct = validatedProduct.Match(
                        some => some,
                        () => null
                    );

                if (newProduct == null)
                {
                    return pendingCart;
                }
                newProduct.Quantity = 1;
                products.Add(newProduct);

            }
            foreach (var product in products)
            {
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
    }
}
