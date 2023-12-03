using System;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Domain.Services;
using ShoppingApp.Domain.Workflows;

namespace ShoppingApp.API.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CartController : Controller
    {
        private readonly ShoppingAppDbContext _dbContext;
        public CartController(ShoppingAppDbContext context)
        {
            _dbContext = context;
        }

        [HttpPost("product")]
        [Authorize]
        public async Task<IActionResult> AddProductroduct(string productCode)
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValue))
            {

                string token = authHeaderValue.ToString().Replace("Bearer ", "");
                var result = TokenService.ExtractAccountID(token);
                string accountID = result.Match(
                        some => some,
                        () => ""
                    );

                AddProductsWorkflow workflow = new(_dbContext);
                await workflow.Execute(accountID, productCode);
                return StatusCode(201);
            }

            return StatusCode(404);
        }
    }
}

