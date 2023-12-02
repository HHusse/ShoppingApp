using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
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
        public static ICart AddProductToCart(EmptyCart emptyCart, string product)
        {
            PendingCart pendingCart = new PendingCart();
            pendingCart.products.Add(product);
            return pendingCart;
        }

        public static ICart AddProductToCart(PendingCart pendingCart, string product)
        {
            pendingCart.products.Add(product);
            return pendingCart;
        }

        public static ICart RemoveProductFromCart(PendingCart pendingCart, string product)
        {
            pendingCart.products.Remove(product);
            return pendingCart;
        }

        public async Task<ICart> ValidateCart(PendingCart pendingCart)
        {
            List<Product> products = new List<Product>();
            foreach (var product in pendingCart.products)
            {
                var validatedProduct = await productService.ValidateProduct(product);

                Product newProduct = validatedProduct.Match(
                        some => some,
                        () => null
                    );

                if (newProduct == null)
                {
                    return pendingCart;
                }
                products.Add(newProduct);

            }
            ValidatedCart validatedCart = new(products);
            return validatedCart;

        }
    }
}
