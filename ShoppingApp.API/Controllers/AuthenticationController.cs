using System;
using Data;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Workflows;

namespace ShoppingApp.API.Controllers
{
    [Route("/api")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly ShoppingAppDbContext _dbContext;
        public AuthenticationController(ShoppingAppDbContext context)
        {
            _dbContext = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string lastName, string firstName, string email, string password)
        {
            Account newAccount = new Account(lastName, firstName, email.ToLower(), password);
            RegisterWorkflow workflow = new(_dbContext);
            GeneralWorkflowResponse res = await workflow.Execute(newAccount);

            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            LoginWorkflow workflow = new(_dbContext);
            var result = await workflow.Execute(email.ToLower(), password);
            string token = result.Match(
                some => some,
                () => ""
            );

            if (String.IsNullOrEmpty(token))
            {
                return StatusCode(404, new { access = false });
            }

            return StatusCode(200, new { token = token, access = true });
        }
    }
}

