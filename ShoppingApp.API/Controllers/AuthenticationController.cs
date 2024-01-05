using System;
using System.ComponentModel.DataAnnotations;
using Data;
using Microsoft.AspNetCore.Mvc;
using ShoppingApp.Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;
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

        public class RegistrationRequest
        {
            [Required]
            public string? LastName { get; set; }
            [Required]
            public string? FirstName { get; set; }
            [Required]
            public string? Email { get; set; }
            [Required]
            public string? PhoneNumber { get; set; }
            [Required]
            public string? Address { get; set; }
            [Required]
            public string? Password { get; set; }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            Account newAccount = new Account(request.LastName, request.FirstName, request.Email.ToLower(), request.PhoneNumber, request.Address, request.Password);
            RegisterWorkflow workflow = new(dbContextFactory);
            GeneralWorkflowResponse res = await workflow.Execute(newAccount);

            return res.Success ? StatusCode(res.StatusCode) : StatusCode(res.StatusCode, new { message = res.Message });
        }

        public class LoginRequest
        {
            [Required]
            public string? Email { get; set; }
            [Required]
            public string? Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            LoginWorkflow workflow = new(dbContextFactory);
            var result = await workflow.Execute(loginRequest.Email.ToLower(), loginRequest.Password);
            string token = result.Match(
                some => some,
                () => ""
            );

            if (String.IsNullOrEmpty(token))
            {
                return StatusCode(401, new { access = false });
            }

            return StatusCode(200, new { token = token, access = true });
        }

        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var authorizationHeader = Request.Headers.Authorization;
            string token = authorizationHeader.ToString().Replace("Bearer ", "");
            var result = TokenService.ExtractAccountID(token);
            string accountID = result.Match(
                    some => some,
                    () => ""
                );

            LogoutWorkflow workflow = new();
            await workflow.Execute(accountID);
            return StatusCode(200);
        }
    }
}

