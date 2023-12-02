using System;
using Azure.Core;
using Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.Services;

namespace ShoppingApp.Domain.Workflows
{
    public class RegisterWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public RegisterWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Execute(Account newAccount)
        {
            newAccount.Password = BCrypt.Net.BCrypt.HashPassword(newAccount.Password);
            AccountService service = new(newAccount, _dbContext);
            return await service.RegisterAccount();
        }
    }
}

