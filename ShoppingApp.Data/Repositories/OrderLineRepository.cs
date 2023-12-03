using Data;
using Microsoft.Identity.Client;
using ShoppingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApp.Data.Repositories
{
    public class OrderLineRepository
    {
        private readonly ShoppingAppDbContext _dbContext;

        public OrderLineRepository(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddProductLine(int quantity, double price, string orderHeaderUid, string productUid)
        {
            string uid = Guid.NewGuid().ToString();

            OrderLineDTO orderLineDTO = new OrderLineDTO
            {
                Uid = uid,
                Quantity = quantity,
                Price = price,
                ProductUid = productUid,
                OrderHeaderUid = orderHeaderUid
            };

            await _dbContext.OrderLines.AddAsync(orderLineDTO);
            _dbContext.SaveChanges();
        }
    }
}
