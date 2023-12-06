using System;
using Azure.Core;
using Data;
using LanguageExt;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.Services;

namespace ShoppingApp.Domain.Workflows
{
    public class LoginWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public LoginWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Option<string>> Execute(string email, string password)
        {
            AccountService service = new(_dbContext);
            Account account = await service.GetAccount(email);
            if (account is null || !BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                return Option<string>.None;
            }

            string token = TokenService.CreateToken(account);
            return Option<string>.Some(token);
        }
    }
}

