using System;
using System.Text.RegularExpressions;
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

        public async Task<int> Execute(Account newAccount)
        {
            newAccount.Password = BCrypt.Net.BCrypt.HashPassword(newAccount.Password);
            Regex rgx = new("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            if (!rgx.IsMatch(newAccount.Email))
            {
                return 400;
            }
            AccountService service = new(newAccount, _dbContext);
            bool succeded = service.RegisterAccount().Result;
            int result = succeded ? 200 : 500;
            return result;
        }
    }
}

