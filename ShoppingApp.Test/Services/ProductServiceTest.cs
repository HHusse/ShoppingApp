using Data;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Moq.EntityFrameworkCore;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShoppingApp.Domain.Models.Cart;
using static ShoppingApp.Test.Utils;

namespace ShoppingApp.Test.Services
{
    internal class ProductServiceTest
    {
        //aici se va testa:
        //ValidateProduct
        //VerifyQunatity
        //GetQuantity

        private Mock<ShoppingAppDbContext> mockDbContext;
        private List<ProductDTO> products;

        [SetUp]
        public void Setup()
        {
            mockDbContext = new();

            products = new List<ProductDTO>();
            CreateFakeProducts(ref products);
            mockDbContext.Setup(m => m.Products).ReturnsDbSet(products);
        }
        [Test]
        [TestCase("1")]
        public void ValidateProduct_ShouldReturnSome_WhenValidationIsSuccesful(string productCode)
        {
            ProductService productService = new ProductService(mockDbContext.Object);

            var returnedProduct = productService.ValidateProduct(productCode).Result;

            Assert.IsTrue(returnedProduct.IsSome);


        }
        [Test]
        [TestCase("18")]
        public void ValidateProduct_ShouldReturnNone_ValidationFails(string productCode)
        {
            ProductService productService = new ProductService(mockDbContext.Object);

            var returnedProduct = productService.ValidateProduct(productCode).Result;

            Assert.IsTrue(returnedProduct.IsNone);
        }
        [Test]
        [TestCase("1",1)]
        public void VerifyQuant_ShouldReturnTrueIfProductExists_WhenProductExists(string productCode, int verifiedQuant)
        {
            ProductService productService = new ProductService(mockDbContext.Object);

            var returnedQuant = productService.VerifyQunatity(productCode,verifiedQuant).Result;

            Assert.IsTrue(returnedQuant);
        }
        [Test]
        [TestCase("1", 14)]
        public void VerifyQuant_ShouldReturnFalseIfProductDoesNotExists_WhenProductDoesNotExist(string productCode, int verifiedQuant)
        {
            ProductService productService = new ProductService(mockDbContext.Object);

            var returnedQuant = productService.VerifyQunatity(productCode, verifiedQuant).Result;

            Assert.IsFalse(returnedQuant);
        }
        [Test]
        [TestCase("1")]
        public void GetQuant_ShouldReturnQuantity_WhenProductExists(string productCode)
        {
            ProductService productService = new ProductService(mockDbContext.Object);

            var returnedQuant = productService.GetQuantity(productCode).Result;

            Assert.AreEqual(returnedQuant, 10);
        }

    }
}
