using System;
using System.Text.RegularExpressions;
using Azure.Core;
using Data;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Services;

namespace ShoppingApp.Domain.Workflows
{
    public class RegisterWorkflow
    {
        private readonly ShoppingAppDbContext _dbContext;

        public RegisterWorkflow(ShoppingAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GeneralWorkflowResponse> Execute(Account newAccount)
        {
            newAccount.Password = BCrypt.Net.BCrypt.HashPassword(newAccount.Password);
            GeneralWorkflowResponse response = new();
            Regex emailRgx = new("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            Regex phoneNumberRgx = new("^[\\+]?[(]?[0-9]{3}[)]?[-\\s\\.]?[0-9]{3}[-\\s\\.]?[0-9]{4,6}$");

            if (!emailRgx.IsMatch(newAccount.Email!))
            {
                response.Success = false;
                response.Message = "Invalid email";
                response.StatusCode = 400;
                return response;
            }
            if (!phoneNumberRgx.IsMatch(newAccount.PhoneNumber!))
            {
                response.Success = false;
                response.Message = "Invalid phone number";
                response.StatusCode = 400;
                return response;
            }

            AccountService service = new(newAccount, _dbContext);
            bool succeded = service.RegisterAccount().Result;
            if (succeded)
            {
                response.Success = true;
                response.StatusCode = 200;
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong";
                response.StatusCode = 500;
            }
            return response;
        }
    }
}

