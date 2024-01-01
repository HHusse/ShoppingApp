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
            return await _dbContext.SaveChangesAsync() == 0 ? false : true;
        }

        public async Task<AccountDTO> GetAccountByEmail(string? email)
        {
            AccountDTO accountDTO = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            return accountDTO;
        }

        public async Task<AccountDTO> GetAccountById(string? id)
        {
            try
            {
                AccountDTO accountDTO = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Uid == id);
                return accountDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}

