using System;
using CSharp.Choices;
using ShoppingApp.Common.Models;

namespace ShoppingApp.Domain.Models
{
    [AsChoice]
    public static partial class Cart
    {
        public interface ICart { };
        public record EmptyCart() : ICart;
        public class PendingCart : ICart
        {
            public List<string> products = new();
        }
        public record ValidatedCart(IReadOnlyCollection<Product> products) : ICart;
        public record CalculatedCart(IReadOnlyCollection<Product> products, double price) : ICart;
        public record PaidCart(IReadOnlyCollection<Product> products, double finalPrice, DateTime data) : ICart;
    }
}

