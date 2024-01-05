using System;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Common.Models;
using ShoppingApp.Data;
using ShoppingApp.Domain.Workflows;
using ShoppingApp.Events;

namespace ShoppingApp.API.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IDbContextFactory dbContextFactory;
        public ProductController(IDbContextFactory dbContextFactory, IEventSender sender)
        {
            this.dbContextFactory = dbContextFactory;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetProducts()
        {
            GetProdcutsWorkflow workflow = new(dbContextFactory);
            List<Product> products = await workflow.Execute();

            if (products.Count <= 0)
            {
                StatusCode(500, new { message = "Somthing went worng" });
            }
            if (products.Count == 0)
            {
                StatusCode(404, new { message = "No products found in deposit" });
            }
            return StatusCode(200, products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductById([FromQuery] string productCode)
        {
            GetSpecificProductWorkflow workflow = new(dbContextFactory);
            Product product = await workflow.Execute(productCode);

            if (product is null)
            {
                return StatusCode(404, new { message = "The product was not found in deposit" });
            }
            return StatusCode(200, product);
        }
    }
}

