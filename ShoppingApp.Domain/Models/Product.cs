using System;
namespace ShoppingApp.Domain.Models
{
    public class Product
    {
        public Product(string? uid, string? name, string? description, double price)
        {
            Uid = uid;
            Name = name;
            Description = description;
            Price = price;
        }

        string? Uid { get; set; }
        string? Name { get; set; }
        string? Description { get; set; }
        double Price { get; set; }
    }
}

