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

        public string? Uid { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
    }
}

