using System;
using System.Security.Principal;
using Data;
using Microsoft.EntityFrameworkCore;
using ShoppingApp.Data.Models;

namespace ShoppingApp.Data.Repositories
{
    public class AccountRepository
    {
        private readonly ShoppingAppDbContext _dbContext;

        public AccountRepository(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<bool> CreateAccount(AccountDTO newAccount)
        {
            await _dbContext.Accounts.AddAsync(newAccount);
            await _dbContext.SaveChangesAsync();
            return await _dbContext.SaveChangesAsync() == 0 ? false : true;
        }

        public async Task<AccountDTO> GetAccountByEmail(string? email)
        {
            AccountDTO accountDTO = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            return accountDTO;
        }

    }
}

