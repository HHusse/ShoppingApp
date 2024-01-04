using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Moq;
using Moq.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using ShoppingApp.Data.Repositories;
using ShoppingApp.Domain.Mappers;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Test.Utils;

namespace ShoppingApp.Test.Services
{
    public class AccountServicesTest
    {
        private Mock<ShoppingAppDbContext> mockDbContext;
        private List<AccountDTO> accounts;
        [SetUp]
        public void Setup()
        {
            accounts = new List<AccountDTO>();
            CreateFakeAccounts(ref accounts);
            mockDbContext = new();

            mockDbContext.Setup(m => m.Accounts).ReturnsDbSet(accounts);
        }
        [Test]
        [TestCase("Ususan", "Catalin", "cataususan@gmail.com", "+40787537581", "STR.Fratiei", "Password")]
        public void RegisterAccount_ShouldReturnTrue_WhenAccountCreationIsSuccessful(string lastName, string firstName, string email, string phoneNumber, string address, string password)
        {
            // Arrange
            var mockDbContext = new Mock<ShoppingAppDbContext>();
            List<AccountDTO> accounts = new List<AccountDTO>();
            mockDbContext.Setup(m => m.Accounts.AddAsync(It.IsAny<AccountDTO>(), default))
                .Callback<AccountDTO, CancellationToken>((account, _) => accounts.Add(account));
            mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);
            Account newAccount = new(lastName, firstName, email, phoneNumber, address, password);
            AccountService accountService = new(newAccount, mockDbContext.Object);


            // Act
            var result = accountService.RegisterAccount().Result;

            // Assert
            Assert.IsTrue(result);
        }
        [Test]
        [TestCase("Ususan", "Catalin", "cataususan@bad.com", "+40787537581", "STR.Fratiei", "Password")]
        public void RegisterAccount_ShouldReturnFalse_WhenAccountCreationIsSuccessful(string lastName, string firstName, string email, string phoneNumber, string address, string password)
        {
            // Arrange
            var mockDbContext = new Mock<ShoppingAppDbContext>();
            List<AccountDTO> accounts = new List<AccountDTO>();
            mockDbContext.Setup(m => m.Accounts.AddAsync(It.IsAny<AccountDTO>(), default))
                .Callback<AccountDTO, CancellationToken>((account, _) => accounts.Add(account));
            mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(0);
            Account newAccount = new(lastName, firstName, email, phoneNumber, address, password);
            AccountService accountService = new(newAccount, mockDbContext.Object);


            // Act
            var result = accountService.RegisterAccount().Result;

            // Assert
            Assert.IsFalse(result);
        }
        //Testare GetAccount
        [Test]
        [TestCase("user2@gmail.com")]
        public void GetAccount_ShouldPass_WhenItGetsAccount(string email)
        {
            AccountService accountService = new AccountService(mockDbContext.Object);
            var Account = accountService.GetAccount(email).Result;
            Assert.IsNotNull(Account);
            
        }
        [Test]
        [TestCase("user2@gmailwrongmain.com")]
        public void GetAccount_ShouldFail_WhenItGetsAccountThatDoesNotExist(string email)
        {
            AccountService accountService = new AccountService(mockDbContext.Object);
            var Account = accountService.GetAccount(email).Result;
            Assert.IsNull(Account);

        }


    }
}
