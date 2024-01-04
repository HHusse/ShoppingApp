using Data;
using Moq;
using Moq.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingApp.Domain.Services;
using static ShoppingApp.Domain.Models.Cart;
using static ShoppingApp.Test.Utils;

namespace ShoppingApp.Test.Services
{
    internal class CartServicesTest
    {
        //De testat aici: 
        // AddProductToCart
        // RemoveProductFromCart
        // ValidateCart
        // CalculateCart
        // PayCart
        // CleanUpCart
        private Mock<ShoppingAppDbContext> mockDbContext;
        private List<ProductDTO> products;
        private List<AccountDTO> accounts;
        private PendingCart cartWithProducts;
        private PendingCart cartWithWrongProducts;
        private ValidatedCart cartToCalculate;
        private CalculatedCart cartToPay;
        [SetUp]
        public void Setup()
        {
            mockDbContext = new();

            products = new List<ProductDTO>();
            CreateFakeProducts(ref products);
            mockDbContext.Setup(m => m.Products).ReturnsDbSet(products);

            CartService addingCartService = new CartService(mockDbContext.Object);
            cartWithProducts = new PendingCart();
            addingCartService.AddProductToCart(cartWithProducts, "1");
            addingCartService.AddProductToCart(cartWithProducts, "2");
            addingCartService.AddProductToCart(cartWithProducts, "3");

            cartWithWrongProducts = new PendingCart();
            addingCartService.AddProductToCart(cartWithWrongProducts, "wrongProduct");

            cartToCalculate = new ValidatedCart(CreateDefaultListOfThreeProducts());
            cartToPay = new CalculatedCart(CreateDefaultListOfThreeProducts(), 15);
        }
        [Test]
        public void AddProductToCart_ShouldReturTrue_WhenAccountAddingIsSuccessful()
        {
            //mockuiesc cart 
            //trimit product code 
            //am verificat doar modalitatea aceasta pentru ca avem validateCart care trateaza cazul in care produsele sunt gresite
            PendingCart pendigCart = new PendingCart();
            var mockDbContext = new Mock<ShoppingAppDbContext>();
            CartService cartService = new CartService(mockDbContext.Object);
            bool productsAfterAdding = cartService.AddProductToCart(pendigCart, "productCode").Result;
            Assert.IsTrue(productsAfterAdding);

        }

        [Test]
        [TestCase("1")]
        public void RemoveProductFromCart_ShouldReturnTheListWithProdeuctRemoved_WhenRemovingIsSuccesful(string product_code)
        {
            CartService cartService = new CartService(mockDbContext.Object);
            cartService.RemoveProductFromCart(cartWithProducts, product_code);
            bool containsElement = cartWithProducts.products.Contains(product_code);
            Assert.IsFalse(containsElement);

        }
        [Test]
        public void ValidateCart_ShuouldReturnAValidatedCart_WhenSuccesful()
        {
            CartService cartService = new CartService(mockDbContext.Object);
            ICart testCart = cartService.ValidateCart(cartWithProducts).Result;

            Assert.That(testCart, Is.TypeOf<ValidatedCart>());
        }
        [Test]
        public void ValidateCart_ShouldReturnAPendingCart_WhenFails()
        {
            CartService cartService = new CartService(mockDbContext.Object);
            ICart testCart = cartService.ValidateCart(cartWithWrongProducts).Result;

            Assert.That(testCart, Is.TypeOf<PendingCart>());
        }
        [Test]
        public void CalculateCart_ShouldReturnACalculatedCart_WhenSuccesful()
        {   //primeste validated Cart
            CartService cartService = new CartService(mockDbContext.Object);
            ICart testCart = cartService.CalculateCart(cartToCalculate).Result;

            Assert.That(testCart, Is.TypeOf<CalculatedCart>());

        }
        [Test]
        public void PayCart_ShouldReturnAPaidCart_WhenSuccesful()
        {
            //primeste Calculated Cart
            CartService cartService = new CartService(mockDbContext.Object);
            ICart testCart = cartService.PayCart(cartToPay).Result;

            Assert.That(testCart, Is.TypeOf<PaidCart>());

        }
        [Test]
        public void CleanupCart_ShouldReturnAPendingCart_WhenSuccesful()
        {
            CartService cartService = new CartService(mockDbContext.Object);
            ICart testCart = cartService.CleanUpCart(cartWithProducts).Result;

            Assert.That(testCart, Is.TypeOf<PendingCart>());
        }

    }
}
