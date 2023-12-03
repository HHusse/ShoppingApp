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

        public OrderService(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
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
            OrderHeaderRepository orderHeaderRepository = new(_dbContext);
            OrderLineRepository orderLineRepository = new(_dbContext);
            
            Dictionary<string,ProductInfo> productList = new Dictionary<string,ProductInfo>();
            string headerUid = await orderHeaderRepository.CreateNewOrderHeader(accountID, paidCart.data.ToString(), paidCart.finalPrice);
            foreach(var product in paidCart.products)
            {
                if(productList.ContainsKey(product.Uid))
                {
                    var productInfo = productList[product.Uid];
                    productInfo.Quantity++;
                    productInfo.Price += product.Price;

                    productList[product.Uid] = productInfo;
                }
                else
                {
                    productList.Add(product.Uid,new ProductInfo(1,product.Price));
                }
            }

            foreach(var product in productList)
            {
                await orderLineRepository.AddProductLine(product.Value.Quantity, product.Value.Price, headerUid, product.Key);
            }
        }
    }
}
