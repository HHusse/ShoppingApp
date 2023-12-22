using System;
using Data;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using ShoppingApp.Data.Repositories;
using ShoppingApp.Domain.Mappers;
using ShoppingApp.Domain.Models;

namespace ShoppingApp.Domain.Services
{
    public class AccountService
    {
        private readonly ShoppingAppDbContext _dbContext;
        AccountRepository accountRepository;
        public Account? Account { get; set; }

        public AccountService(Account? account, ShoppingAppDbContext dbContext)
        {
            Account = account;
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            accountRepository = new(_dbContext);
        }

        public AccountService(ShoppingAppDbContext dbContext)
        {
            Account = null;
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            accountRepository = new(_dbContext);
        }

        public async Task<bool> RegisterAccount()
        {
            try
            {
                return await accountRepository.CreateAccount(AccountMapper.MapToAccountDTO(Account!));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Account> GetAccount(string? email) => AccountMapper.MapToAccount(await accountRepository.GetAccountByEmail(email));






    }
}

