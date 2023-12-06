using System;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Domain;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
using ShoppingApp.Domain.Workflows;
using static ShoppingApp.Domain.Models.Cart;

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
            GeneralWorkflowResponse res = await workflow.Execute(accountID, productCode);
            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });
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
            GeneralWorkflowResponse res = await workflow.Execute(accountID, productCode);
            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });

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
            GeneralWorkflowResponse res = await workflow.Execute(accountID);
            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });

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
            GeneralWorkflowResponse res = await workflow.Execute(accountID);
            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });

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
            GeneralWorkflowResponse res = await workflow.Execute(accountID);
            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });

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
            GeneralWorkflowResponse res = await workflow.Execute(accountID);
            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCart()
        {
            var authorizationHeader = Request.Headers.Authorization;
            string token = authorizationHeader.ToString().Replace("Bearer ", "");
            var result = TokenService.ExtractAccountID(token);
            string accountID = result.Match(
                    some => some,
                    () => ""
                );

            GetCartWorkflow workflow = new(_dbContext);
            CartResponse res = await workflow.Execute(accountID);            

            return StatusCode(200, res);

        }
    }
}

