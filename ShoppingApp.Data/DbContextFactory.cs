using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ShoppingApp.Data
{
    public class DbContextFactory : IDbContextFactory
    {
        public ShoppingAppDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShoppingAppDbContext>();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("DBCONNECTIONSTRING"));

            return new ShoppingAppDbContext(optionsBuilder.Options);
        }
    }
}

