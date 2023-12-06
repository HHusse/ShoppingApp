using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Models;

namespace ShoppingApp.Domain.Mappers
{
    public class AccountMapper
    {
        public static Account MapToAccount(AccountDTO accountDTO)
        {
            if (accountDTO == null)
                return null;

            return new Account(
                Guid.Parse(accountDTO.Uid!),
                accountDTO.LastName,
                accountDTO.FirstName,
                accountDTO.Email,
                accountDTO.PhoneNumber,
                accountDTO.Address,
                accountDTO.Password
            );
        }

        public static AccountDTO MapToAccountDTO(Account account)
        {
            if (account == null)
                return null;

            return new AccountDTO
            {
                Uid = account.Uid.ToString(),
                LastName = account.LastName,
                FirstName = account.FirstName,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber,
                Address = account.Address,
                Password = account.Password,
                OrderHeaders = new List<OrderHeaderDTO>()
            };
        }
    }
}
