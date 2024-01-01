using System;
using Data;
using LanguageExt.Pretty;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data;
using ShoppingApp.Data.Models;
using ShoppingApp.Data.Repositories;
using ShoppingApp.Domain.Mappers;
using ShoppingApp.Domain.Models;

namespace ShoppingApp.Domain.Services
{
    public class AccountService
    {
        private readonly IDbContextFactory dbContextFactory;
        AccountRepository accountRepository;
        public Account? Account { get; set; }

        public AccountService(Account? account, IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
            Account = account;
            accountRepository = new(dbContextFactory.CreateDbContext());
        }

        public AccountService(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
            Account = null;
            accountRepository = new(dbContextFactory.CreateDbContext());
        }

        public async Task<bool> RegisterAccount()
        {
            return await accountRepository.CreateAccount(AccountMapper.MapToAccountDTO(Account!));
        }

        public async Task<Account> GetAccountByEmail(string? email) => AccountMapper.MapToAccount(await accountRepository.GetAccountByEmail(email));

        public async Task<Account> GetAccountById(string? id) => AccountMapper.MapToAccount(await accountRepository.GetAccountById(id));

    }
}

