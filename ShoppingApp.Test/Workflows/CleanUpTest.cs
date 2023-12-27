using System;
using Data;
using Moq;
using Moq.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Services;
using ShoppingApp.Domain.Workflows;
using static ShoppingApp.Test.Utils;
using static ShoppingApp.Domain.CartsRepository;
using static ShoppingApp.Domain.Models.Cart;
using ShoppingApp.Domain.ResponseModels;

namespace ShoppingApp.Test.Workflows
{
    public class CleanUpTest
    {
        private Mock<ShoppingAppDbContext> mockDbContext;
        private List<AccountDTO> accounts;
        private List<ProductDTO> products;
        private string accountTestId = "";

        [SetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("SECRETKEY", "HbZ3@e7M&K8#P$2sW5vY9zC*F1tA6gS ");
            accounts = new List<AccountDTO>();
            CreateFakeAccounts(ref accounts);
            products = new List<ProductDTO>();
            CreateFakeProducts(ref products);
            mockDbContext = new();
            mockDbContext.Setup(m => m.Accounts).ReturnsDbSet(accounts);
            mockDbContext.Setup(m => m.Products).ReturnsDbSet(products);

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
        //Products that don't exist
        [TestCase("5")]
        [TestCase("6")]
        [TestCase("7")]
        [TestCase("8")]
        public void CleanUpPendingCart(string productID)
        {
            PendingCart paidCart = new();
            paidCart.products.AddRange(new List<string> { "1", "2", "3", "4" });
            paidCart.products.Add(productID);
            carts.TryAdd(accountTestId, paidCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            CleanUpCartWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId).Result;

            Assert.IsTrue(response.Success);
            var resultCart = (PendingCart)carts.First().Value;
            Assert.That(resultCart.products.Count(), Is.EqualTo(4));
            Assert.False(resultCart.products.Contains(productID));

        }

        public static IEnumerable<TestCaseData> CartTestCases
        {
            get
            {
                yield return new TestCaseData(new ValidatedCart(CreateDefaultListOfThreeProducts()), "Is a validated cart");
                yield return new TestCaseData(new CalculatedCart(CreateDefaultListOfThreeProducts(), 6), "Is a calculated cart");
                yield return new TestCaseData(new EmptyCart(), "Is an empty cart");
                yield return new TestCaseData(new PaidCart(CreateDefaultListOfThreeProducts(), 6, DateTime.Now.Date), "Is a paid cart");
            }
        }

        [Test]
        [TestCaseSource(nameof(CartTestCases))]
        public void CleanUpDiffrentCarts(ICart cart, string expectedMessage)
        {
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            CleanUpCartWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId).Result;

            Assert.IsFalse(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf(cart.GetType()));
            Assert.That(response.Message, Is.EqualTo(expectedMessage));
        }
    }
}

