using System;
using Azure.Core;
using Data;
using LanguageExt;
using ShoppingApp.Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.Services;

namespace ShoppingApp.Domain.Workflows
{
    public class LoginWorkflow
    {
        private readonly IDbContextFactory dbContextFactory;

        public LoginWorkflow(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public async Task<Option<string>> Execute(string email, string password)
        {
            AccountService service = new(dbContextFactory);
            Account account = await service.GetAccountByEmail(email);
            if (account is null || !BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                return Option<string>.None;
            }

            string token = TokenService.CreateToken(account);
            return Option<string>.Some(token);
        }
    }
}

