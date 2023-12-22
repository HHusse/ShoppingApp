using System;
using ShoppingApp.Data.Models;

namespace ShoppingApp.Test.Utils
{
    public static class AccountTestUtils
    {
        public static void CreateFakeAccounts(ref List<AccountDTO> accounts)
        {
            accounts.Add(new AccountDTO
            {
                Uid = Guid.NewGuid().ToString(),
                LastName = "Vahid",
                FirstName = "Hossein",
                Email = "husse@gmail.com",
                PhoneNumber = "+40712345678",
                Address = "STR. Test",
                Password = BCrypt.Net.BCrypt.HashPassword("1234")
            });

            accounts.Add(new AccountDTO
            {
                Uid = Guid.NewGuid().ToString(),
                LastName = "user2",
                FirstName = "user2",
                Email = "user2@gmail.com",
                PhoneNumber = "+40712345678",
                Address = "STR. Test2",
                Password = BCrypt.Net.BCrypt.HashPassword("1234")
            });

            accounts.Add(new AccountDTO
            {
                Uid = Guid.NewGuid().ToString(),
                LastName = "user3",
                FirstName = "user3",
                Email = "user3@gmail.com",
                PhoneNumber = "+40712345678",
                Address = "STR. Test3",
                Password = BCrypt.Net.BCrypt.HashPassword("1234")
            });
        }
    }
}

