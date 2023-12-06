using System;
using System.Security.Cryptography;

namespace ShoppingApp.Domain.Models
{
    public class Product
    {
        public Product()
        {
            Uid = "";
            Name = "";
            Description = "";
            Quantity = -1;
            Price = -1;
        }

        public Product(string uid, string name, string description, int quantity, double price)
        {
            Uid = uid;
            Name = name;
            Description = description;
            Quantity = quantity;
            Price = price;
        }

        public string Uid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}

