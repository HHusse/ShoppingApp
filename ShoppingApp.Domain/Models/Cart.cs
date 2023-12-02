using System;
namespace ShoppingApp.Domain.Models
{
    public class Cart
    {
        Guid Uid { get; set; }
        string? AccountId { get; set; }
        List<Product> Products { get; set; } = new();
    }
}

