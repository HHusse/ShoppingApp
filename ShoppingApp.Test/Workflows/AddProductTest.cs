using Data;
using Moq;
using Moq.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using ShoppingApp.Domain.Workflows;
using static ShoppingApp.Test.Utils;
using static ShoppingApp.Domain.CartsRepository;
using static ShoppingApp.Domain.Models.Cart;
using ShoppingApp.Domain.Models;

namespace ShoppingApp.Test.Workflows
{
    public class AddProductTest
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
        public void AddProduct(string productID)
        {

            AddProductsWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;

            Assert.IsTrue(response.Success);
            Assert.That(carts.Count(), Is.EqualTo(1));
            Assert.That(carts.First().Value, Is.TypeOf<PendingCart>());
        }

        [Test]
        public void AddNewProductToEmptyCart()
        {
            string productID = "newProduct";
            EmptyCart paidCart = new();
            carts.TryAdd(accountTestId, paidCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            AddProductsWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;


            Assert.IsTrue(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PendingCart>());
            var resultCart = (PendingCart)carts.First().Value;
            Assert.IsTrue(resultCart.products.Contains(productID));
        }

        [Test]
        public void AddNewProductToPendingCart()
        {
            string productID = "newProduct";
            PendingCart paidCart = new();
            carts.TryAdd(accountTestId, paidCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            AddProductsWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;


            Assert.IsTrue(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PendingCart>());
            var resultCart = (PendingCart)carts.First().Value;
            Assert.IsTrue(resultCart.products.Contains(productID));
        }

        [Test]
        public void AddNewProductToValidatedCart()
        {
            string productID = "newProduct";
            ValidatedCart paidCart = new(new List<Product>());
            carts.TryAdd(accountTestId, paidCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            AddProductsWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;


            Assert.IsTrue(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PendingCart>());
            var resultCart = (PendingCart)carts.First().Value;
            Assert.IsTrue(resultCart.products.Contains(productID));
        }

        [Test]
        public void AddNewProductToCalculatedCart()
        {
            string productID = "newProduct";
            CalculatedCart paidCart = new(new List<Product>(), 0);
            carts.TryAdd(accountTestId, paidCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            AddProductsWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, productID).Result;


            Assert.IsTrue(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PendingCart>());
            var resultCart = (PendingCart)carts.First().Value;
            Assert.IsTrue(resultCart.products.Contains(productID));
        }

        [Test]
        public void TryToAddNewProductToPaidCart()
        {
            PaidCart paidCart = new(new List<Product>(), 0, DateTime.Now);
            carts.TryAdd(accountTestId, paidCart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            AddProductsWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(accountTestId, "newProduct").Result;

            Assert.IsFalse(response.Success);
            Assert.That(carts.First().Value, Is.TypeOf<PaidCart>());
        }
    }
}

