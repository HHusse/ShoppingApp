using System;
using Data;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Workflows;

namespace ShoppingApp.API.Controllers
{
    [Route("/api")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IDbContextFactory dbContextFactory;
        public AuthenticationController(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string lastName, string firstName, string email, string phoneNumber, string address, string password)
        {
            Account newAccount = new Account(lastName, firstName, email.ToLower(), phoneNumber, address, password);
            RegisterWorkflow workflow = new(dbContextFactory);
            GeneralWorkflowResponse res = await workflow.Execute(newAccount);

            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            LoginWorkflow workflow = new(dbContextFactory);
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

