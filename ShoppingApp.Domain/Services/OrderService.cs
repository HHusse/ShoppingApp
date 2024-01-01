using Data;
using LanguageExt.ClassInstances;
using ShoppingApp.Common.Models;
using ShoppingApp.Data;
using ShoppingApp.Data.Repositories;
using ShoppingApp.Domain.Models;
using ShoppingApp.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingApp.Domain.Models.Cart;

namespace ShoppingApp.Domain.Services
{
    public class OrderService
    {
        private readonly IDbContextFactory dbContextFactory;
        OrderHeaderRepository orderHeaderRepository;
        OrderLineRepository orderLineRepository;
        ProductRepository productRepository;
        AccountService accountService;
        IEventSender sender;

        public OrderService(IDbContextFactory dbContextFactory, IEventSender sender)
        {
            this.dbContextFactory = dbContextFactory;
            this.sender = sender;
            orderHeaderRepository = new(dbContextFactory.CreateDbContext());
            orderLineRepository = new(dbContextFactory.CreateDbContext());
            productRepository = new(dbContextFactory.CreateDbContext());
            accountService = new(dbContextFactory);
        }

        public async Task PlaceOrder(string accountID, PaidCart paidCart)
        {
            Account account = await accountService.GetAccountById(accountID);
            string headerUid = await orderHeaderRepository.CreateNewOrderHeader(accountID, paidCart.data.ToString(), paidCart.finalPrice);
            foreach (var product in paidCart.products)
            {
                await productRepository.RemoveQuantity(product.Uid, product.Quantity);
                await orderLineRepository.AddProductLine(product.Quantity, product.Price, headerUid, product.Uid);
            }
            InvoiceDetails invoice = new(account.Email!, headerUid, (account.FirstName + " " + account.LastName), "ShoppingExpress", account.Address!, account.PhoneNumber!, (List<Product>)paidCart.products);
            await sender.SendAsync("invoicequeue", invoice);
        }
    }
}
