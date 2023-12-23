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
    public class GetCartTest
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
        public void TestWithEmptyCart()
        {
            EmptyCart cart = new EmptyCart();
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            GetCartWorkflow workflow = new(mockDbContext.Object);
            CartResponse response = workflow.Execute(accountTestId).Result;

            Assert.That(response.State, Is.EqualTo("empty"));
        }

        [Test]
        public void TestWithPendingCart()
        {
            PendingCart cart = new PendingCart();
            cart.products.Add("1");
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            GetCartWorkflow workflow = new(mockDbContext.Object);
            CartResponse response = workflow.Execute(accountTestId).Result;

            Assert.That(response.State, Is.EqualTo("pending"));

            var cartType = response.Cart.GetType();
            var productsProperty = cartType.GetProperty("products");
            Assert.IsNotNull(productsProperty);

            var products = productsProperty.GetValue(response.Cart, null) as IEnumerable<dynamic>;
            Assert.IsNotNull(products);

            var product = products.First();
            Assert.IsNotNull(product);
            var productType = product.GetType();
            var idProperty = productType.GetProperty("id");
            Assert.IsNotNull(idProperty);
            var idValue = idProperty.GetValue(product) as string;
            bool productExists = idValue == "1";

            Assert.IsTrue(productExists);
        }

        [Test]
        public void TestWithValidatedCart()
        {
            ValidatedCart cart = new ValidatedCart(CreateDefaultListOfThreeProducts());
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            GetCartWorkflow workflow = new(mockDbContext.Object);
            CartResponse response = workflow.Execute(accountTestId).Result;

            Assert.That(response.State, Is.EqualTo("validated"));
            var cartType = response.Cart.GetType();
            var productsProperty = cartType.GetProperty("products");
            Assert.IsNotNull(productsProperty);

            var products = productsProperty.GetValue(response.Cart, null) as IEnumerable<dynamic>;
            Assert.IsNotNull(products);

            bool productExists = products.Any(p => p.Uid == "1");
            Assert.IsTrue(productExists);

            productExists = products.Any(p => p.Uid == "2");
            Assert.IsTrue(productExists);

            productExists = products.Any(p => p.Uid == "3");
            Assert.IsTrue(productExists);
        }

        [Test]
        public void TestWithCalculatedCart()
        {
            CalculatedCart cart = new CalculatedCart(CreateDefaultListOfThreeProducts(), 6);
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            GetCartWorkflow workflow = new(mockDbContext.Object);
            CartResponse response = workflow.Execute(accountTestId).Result;

            Assert.That(response.State, Is.EqualTo("calculated"));
            var cartType = response.Cart.GetType();
            var productsProperty = cartType.GetProperty("products");
            Assert.IsNotNull(productsProperty);

            var products = productsProperty.GetValue(response.Cart, null) as IEnumerable<dynamic>;
            Assert.IsNotNull(products);

            bool productExists = products.Any(p => p.Uid == "1");
            Assert.IsTrue(productExists);

            productExists = products.Any(p => p.Uid == "2");
            Assert.IsTrue(productExists);

            productExists = products.Any(p => p.Uid == "3");
            Assert.IsTrue(productExists);

            var priceProprety = cartType.GetProperty("price");
            Assert.IsNotNull(priceProprety);

            var price = priceProprety.GetValue(response.Cart, null);
            Assert.IsNotNull(price);
            Assert.That(price, Is.EqualTo(6));
        }

        [Test]
        public void TestWithPaidCart()
        {
            DateTime timeCreated = DateTime.Now;
            PaidCart cart = new PaidCart(CreateDefaultListOfThreeProducts(), 6, timeCreated);
            carts.TryAdd(accountTestId, cart);
            if (carts.Count() != 1)
            {
                Assert.Fail();
            }

            GetCartWorkflow workflow = new(mockDbContext.Object);
            CartResponse response = workflow.Execute(accountTestId).Result;

            Assert.That(response.State, Is.EqualTo("paid"));
            var cartType = response.Cart.GetType();
            var productsProperty = cartType.GetProperty("products");
            Assert.IsNotNull(productsProperty);

            var products = productsProperty.GetValue(response.Cart, null) as IEnumerable<dynamic>;
            Assert.IsNotNull(products);

            bool productExists = products.Any(p => p.Uid == "1");
            Assert.IsTrue(productExists);

            productExists = products.Any(p => p.Uid == "2");
            Assert.IsTrue(productExists);

            productExists = products.Any(p => p.Uid == "3");
            Assert.IsTrue(productExists);

            var priceProprety = cartType.GetProperty("finalPrice");
            Assert.IsNotNull(priceProprety);

            var price = priceProprety.GetValue(response.Cart, null);
            Assert.IsNotNull(price);
            Assert.That(price, Is.EqualTo(6));

            var dateProprety = cartType.GetProperty("data");
            Assert.IsNotNull(priceProprety);

            var data = dateProprety.GetValue(response.Cart, null);
            Assert.IsNotNull(data);
            Assert.That(data, Is.EqualTo(timeCreated));
        }
    }
}

