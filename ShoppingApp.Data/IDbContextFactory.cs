using System;
using Data;

namespace ShoppingApp.Data
{
    public interface IDbContextFactory
    {
        ShoppingAppDbContext CreateDbContext();
    }
}

