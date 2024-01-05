using System;
using Microsoft.Identity.Client;

namespace ShoppingApp.Domain.Workflows
{
    public class LogoutWorkflow
    {
        public LogoutWorkflow()
        {
        }
        public async Task Execute(string accountID)
        {
            await CartsRepository.RemoveCart(accountID);
        }
    }
}

