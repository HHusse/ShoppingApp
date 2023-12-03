using Data;
using LanguageExt.ClassInstances;
using ShoppingApp.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Services
{
    public class OrderService
    {
        private readonly ShoppingAppDbContext _dbContext;
        OrderHeaderRepository orderHeaderRepository;
        OrderLineRepository orderLineRepository;
        ProductRepository productRepository;

        public OrderService(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
            orderHeaderRepository = new(_dbContext);
            orderLineRepository = new(_dbContext);
            productRepository = new(_dbContext);
        }

        private struct ProductInfo
        {
            public int Quantity { get; set; }
            public double Price { get; set; }

            public ProductInfo(int quantity, double price)
            {
                Quantity = quantity;
                Price = price;
            }
        }
        public async Task PlaceOrder(string accountID, PaidCart paidCart)
        {
            string headerUid = await orderHeaderRepository.CreateNewOrderHeader(accountID, paidCart.data.ToString(), paidCart.finalPrice);
            foreach (var product in paidCart.products)
            {
                await orderLineRepository.AddProductLine(product.Quantity, product.Price, headerUid, product.Uid);
                await productRepository.RemoveQuantity(product.Uid, product.Quantity);
            }

        }
    }
}
