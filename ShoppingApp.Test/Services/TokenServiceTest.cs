using Data;
using Moq;
using Moq.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Mappers;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingApp.Test.Utils;

namespace ShoppingApp.Test.Services
{
    internal class TokenServiceTest
    {
        private Mock<ShoppingAppDbContext> mockDbContext;
        private List<ProductDTO> products;
        private List<AccountDTO> accounts;
       
        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("SECRETKEY", "HbZ3@e7M&K8#P$2sW5vY9zC*F1tA6gS ");
            mockDbContext = new();

            accounts = new List<AccountDTO>();
            CreateFakeAccounts(ref accounts);
            mockDbContext.Setup(m => m.Accounts).ReturnsDbSet(accounts);

        }
        [Test]
        public void CreateToken_ReturnsToken_IfItWorks()
        {
            var accountDTO = mockDbContext.Object.Accounts.First();
            Account testAccount = AccountMapper.MapToAccount(accountDTO);
            var token = TokenService.CreateToken(testAccount);

            Assert.IsNotNull(token);
        }
        [Test]
        public void ExtractAccountId_ReturnsSomne_IfSuccesful()
        {
            var accountDTO = mockDbContext.Object.Accounts.First();
            Account testAccount = AccountMapper.MapToAccount(accountDTO);
            var token = TokenService.CreateToken(testAccount);

            var accountID = TokenService.ExtractAccountID(token);

            Assert.IsTrue(accountID.IsSome);

        }
        [Test]
        [TestCase("wrongToken")]
        public void ExtractAcountId_ShouldReturnNone_IfFails(string token)
        {

            var fakeToken = CreateFakeToken();

            var badAccountID = TokenService.ExtractAccountID(fakeToken);

            Assert.IsTrue(badAccountID.IsNone);
        }
    }
}
