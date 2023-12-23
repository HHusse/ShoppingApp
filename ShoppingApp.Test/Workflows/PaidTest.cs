using System;
using Data;
using Moq;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Services;
using ShoppingApp.Domain.Workflows;
using static ShoppingApp.Test.Utils;
using static ShoppingApp.Domain.CartsRepository;
using static ShoppingApp.Domain.Models.Cart;
using Moq.EntityFrameworkCore;
using ShoppingApp.Domain.ResponseModels;

namespace ShoppingApp.Test.Workflows
{
    public class PaidTest
    {
        private Mock<ShoppingAppDbContext> mockDbContext;
        private List<AccountDTO> accounts;
        private string accountTestId = "";

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("SECRETKEY", "HbZ3@e7M&K8#P$2sW5vY9zC*F1tA6gS ");
            accounts = new List<AccountDTO>();
            CreateFakeAccounts(ref accounts);
            mockDbContext = new();
            mockDbContext.Setup(m => m.Accounts).ReturnsDbSet(accounts);

            LoginWorkflow workflow = new(mockDbContext.Object);
            var result = workflow.Execute("husse@gmail.com", "1234").Result;
            string token = result.Match(
                some => some,
                () => ""
            );

            if (String.IsNullOrEmpty(token))
            {
                Assert.Fail();
            }

            TokenService.ExtractAccountID(token).IfSome(id => accountTestId = id);
            if (String.IsNullOrEmpty(accountTestId))
            {
                Assert.Fail();
            }
            carts.Clear();
        }

        [Test]
        public void PayCalculatedCart()
        {
            CalculatedCart cart = new CalculatedCart(CreateDefaultListOfThreeProducts(), 6);
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            PayCartWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId).Result;

            Assert.IsTrue(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PaidCart>());
            var resultCart = (PaidCart)carts.First().Value;
            Assert.That(resultCart.products.Count(), Is.EqualTo(3));
            Assert.That(resultCart.finalPrice, Is.EqualTo(6));
        }

        public static IEnumerable<TestCaseData> CartTestCases
        {
            get
            {
                yield return new TestCaseData(new PendingCart(), "Is a pending cart");
                yield return new TestCaseData(new ValidatedCart(CreateDefaultListOfThreeProducts()), "Is a validated cart");
                yield return new TestCaseData(new EmptyCart(), "Is an empty cart");
                yield return new TestCaseData(new PaidCart(CreateDefaultListOfThreeProducts(), 6, DateTime.Now.Date), "Is a paid cart");
            }
        }

        [Test]
        [TestCaseSource(nameof(CartTestCases))]
        public void TryToCalculateDiffrentCarts(ICart cart, string expectedMessage)
        {
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            PayCartWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId).Result;

            Assert.IsFalse(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf(cart.GetType()));
            Assert.That(response.Message, Is.EqualTo(expectedMessage));
        }
    }
}

