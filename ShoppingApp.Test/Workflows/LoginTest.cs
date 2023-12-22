using Data;
using LanguageExt;
using Moq;
using Moq.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Workflows;
using static ShoppingApp.Test.Utils.AccountTestUtils;

namespace ShoppingApp.Test.Workflows
{
    public class LoginTest
    {
        private Mock<ShoppingAppDbContext> mockDbContext;
        private List<AccountDTO> accounts;

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("SECRETKEY", "HbZ3@e7M&K8#P$2sW5vY9zC*F1tA6gS ");
            accounts = new List<AccountDTO>();
            CreateFakeAccounts(ref accounts);
            mockDbContext = new();

            mockDbContext.Setup(m => m.Accounts).ReturnsDbSet(accounts);
        }

        [Test]
        [TestCase("husse@gmail.com", "1234")]
        [TestCase("user2@gmail.com", "1234")]
        [TestCase("user3@gmail.com", "1234")]
        public void LoginValidUser(string email, string password)
        {
            LoginWorkflow workflow = new(mockDbContext.Object);
            Option<string> response = workflow.Execute(email, password).Result;

            Assert.IsFalse(response.IsNone);
        }

        [Test]
        [TestCase("husse@gmail.com", "12345")]
        [TestCase("user2@gmail.com", "12345")]
        [TestCase("user3@gmail.com", "12345")]
        public void LoginValidUserWrongPassword(string email, string password)
        {
            LoginWorkflow workflow = new(mockDbContext.Object);
            Option<string> response = workflow.Execute(email, password).Result;

            Assert.IsTrue(response.IsNone);
        }

        [Test]
        [TestCase("husse123@gmail.com", "12345")]
        [TestCase("user22@gmail.com", "12345")]
        [TestCase("user33@gmail.com", "12345")]
        public void TryLoginUnregisteredUsers(string email, string password)
        {
            LoginWorkflow workflow = new(mockDbContext.Object);
            Option<string> response = workflow.Execute(email, password).Result;

            Assert.IsTrue(response.IsNone);
        }
    }
}

