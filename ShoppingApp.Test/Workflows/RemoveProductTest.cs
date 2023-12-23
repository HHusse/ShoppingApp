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
using ShoppingApp.Domain.Models;
using LanguageExt.ClassInstances;
using NUnit.Framework;

namespace ShoppingApp.Test.Workflows
{
    public class RemoveProductTest
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
        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("4")]
        public void RemoveSimpleProduct(string productID)
        {
            PendingCart paidCart = new();
            paidCart.products.Add(productID);
            carts.TryAdd(accountTestId, paidCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            RemoveProductWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;

            Assert.IsTrue(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PendingCart>());
            var cart = (PendingCart)carts.First().Value;
            Assert.That(cart.products.Count(), Is.EqualTo(0));
        }

        public static IEnumerable<TestCaseData> CartTestCases
        {
            get
            {
                yield return new TestCaseData(new ValidatedCart(CreateDefaultListOfThreeProducts()), "1");
                yield return new TestCaseData(new CalculatedCart(CreateDefaultListOfThreeProducts(), 6), "2");
            }
        }

        [Test]
        [TestCaseSource(nameof(CartTestCases))]
        public void RemoveProductFromCartTestCases(ICart cart, string productID)
        {
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }
            RemoveProductWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;

            Assert.IsTrue(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PendingCart>());
            var resultCart = (PendingCart)carts.First().Value;
            Assert.That(resultCart.products.Count(), Is.EqualTo(2));
            Assert.IsFalse(resultCart.products.Contains(productID));
        }

        [Test]
        public void TryToRemoveProductFromEmptyCart()
        {
            string productID = "1";
            EmptyCart emptyCart = new();
            carts.TryAdd(accountTestId, emptyCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            RemoveProductWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;

            Assert.IsFalse(response.Success);
            Assert.That(response.Message, Is.EqualTo("Is an empty cart"));
        }

        [Test]
        public void TryToRemoveProductThatDosentExistInCart()
        {
            string productID = "3";
            PendingCart pendingCart = new();
            pendingCart.products.Add("1");
            pendingCart.products.Add("2");
            carts.TryAdd(accountTestId, pendingCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            RemoveProductWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;

            Assert.IsFalse(response.Success);
            Assert.That(response.Message, Is.EqualTo("The product could not be deleted because it does not exist in the cart"));
            var resultCart = (PendingCart)carts.First().Value;
            Assert.That(resultCart.products.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TryToRemoveProductFromPendingCartThatDoesNotContainAnyProducts()
        {
            string productID = "3";
            PendingCart pendingCart = new();
            carts.TryAdd(accountTestId, pendingCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            RemoveProductWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;

            Assert.IsFalse(response.Success);
            Assert.That(response.Message, Is.EqualTo("The product could not be deleted because it does not exist in the cart"));
            var resultCart = (PendingCart)carts.First().Value;
            Assert.That(resultCart.products.Count(), Is.EqualTo(0));
        }

        [Test]
        public void TryToRemoveProductFromPaidCart()
        {
            string productID = "3";
            PaidCart paidCart = new(CreateDefaultListOfThreeProducts(), 6, DateTime.Now);
            carts.TryAdd(accountTestId, paidCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            RemoveProductWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;

            Assert.IsFalse(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PaidCart>());
            Assert.That(response.Message, Is.EqualTo("Is a paid cart"));
        }
    }
}

