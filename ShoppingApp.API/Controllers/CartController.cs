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
            var authorizationHeader = Request.Headers.Authorization;
            string token = authorizationHeader.ToString().Replace("Bearer ", "");
            var result = TokenService.ExtractAccountID(token);
            string accountID = result.Match(
                    some => some,
                    () => ""
                );

            AddProductsWorkflow workflow = new(_dbContext);
            bool succeded = await workflow.Execute(accountID, productCode);
            return succeded ? StatusCode(201) : StatusCode(403);

        }

        [HttpDelete("product")]
        [Authorize]
        public async Task<IActionResult> DeleteProductroduct(string productCode)
        {
            var authorizationHeader = Request.Headers.Authorization;
            string token = authorizationHeader.ToString().Replace("Bearer ", "");
            var result = TokenService.ExtractAccountID(token);
            string accountID = result.Match(
                    some => some,
                    () => ""
                );

            RemoveProductWorkflow workflow = new(_dbContext);
            bool succeded = await workflow.Execute(accountID, productCode);
            return succeded ? StatusCode(201) : StatusCode(403);

        }

        [HttpPost("pay")]
        [Authorize]
        public async Task<IActionResult> PayCart()
        {
            var authorizationHeader = Request.Headers.Authorization;
            string token = authorizationHeader.ToString().Replace("Bearer ", "");
            var result = TokenService.ExtractAccountID(token);
            string accountID = result.Match(
                    some => some,
                    () => ""
                );

            PayCartWorkflow workflow = new(_dbContext);
            bool succeded = await workflow.Execute(accountID);
            return succeded ? StatusCode(201) : StatusCode(403);

        }

        [HttpPost("validate")]
        [Authorize]
        public async Task<IActionResult> ValidateCart()
        {
            var authorizationHeader = Request.Headers.Authorization;
            string token = authorizationHeader.ToString().Replace("Bearer ", "");
            var result = TokenService.ExtractAccountID(token);
            string accountID = result.Match(
                    some => some,
                    () => ""
                );

            ValidateCartWorkflow workflow = new(_dbContext);
            bool succeded = await workflow.Execute(accountID);
            return succeded ? StatusCode(201) : StatusCode(403);

        }

        [HttpPost("calculate")]
        [Authorize]
        public async Task<IActionResult> CalculateCart()
        {
            var authorizationHeader = Request.Headers.Authorization;
            string token = authorizationHeader.ToString().Replace("Bearer ", "");
            var result = TokenService.ExtractAccountID(token);
            string accountID = result.Match(
                    some => some,
                    () => ""
                );

            CalculateCartWorkflow workflow = new(_dbContext);
            bool succeded = await workflow.Execute(accountID);
            return succeded ? StatusCode(201) : StatusCode(403);

        }

        [HttpPost("order")]
        [Authorize]
        public async Task<IActionResult> PlaceOrder()
        {
            var authorizationHeader = Request.Headers.Authorization;
            string token = authorizationHeader.ToString().Replace("Bearer ", "");
            var result = TokenService.ExtractAccountID(token);
            string accountID = result.Match(
                    some => some,
                    () => ""
                );

            PlaceOrderWorkflow workflow = new(_dbContext);
            bool succeded = await workflow.Execute(accountID);
            return succeded ? StatusCode(201) : StatusCode(403);

        }
    }
}

