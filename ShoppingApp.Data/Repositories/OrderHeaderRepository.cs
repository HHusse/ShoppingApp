using Data;
using ShoppingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApp.Data.Repositories
{
    public class OrderHeaderRepository
    {
        private readonly ShoppingAppDbContext _dbContext;

        public OrderHeaderRepository(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> CreateNewOrderHeader(string accountID, string date, double total)
        {
            string uid = Guid.NewGuid().ToString();
            OrderHeaderDTO orderHeader = new OrderHeaderDTO
            {
                Uid = uid,
                AccountId = accountID,
                Date = date,
                Total = total,
            };
            await _dbContext.OrderHeaders.AddAsync(orderHeader);
            _dbContext.SaveChanges();
            return uid;
        }
    }
}
