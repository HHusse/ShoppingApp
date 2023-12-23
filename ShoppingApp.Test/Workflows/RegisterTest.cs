using System;
using Data;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Models;
using ShoppingApp.Domain.ResponseModels;
using ShoppingApp.Domain.Workflows;

namespace ShoppingApp.Test.Workflows
{
    public class RegisterTest
    {
        private Mock<ShoppingAppDbContext> mockDbContext;
        private List<AccountDTO> accounts;

        [SetUp]
        public void Setup()
        {
            accounts = new List<AccountDTO>();
            mockDbContext = new();

            mockDbContext.Setup(m => m.Accounts).ReturnsDbSet(accounts);
            mockDbContext.Setup(m => m.Accounts.AddAsync(It.IsAny<AccountDTO>(), default))
                    .Callback<AccountDTO, CancellationToken>((account, _) => accounts.Add(account));
        }

        [Test]
        [TestCase("Vahid", "Hossein", "husse@gmail.com", "+40712345678", "STR.Test", "Password")]
        [TestCase("Nume", "Prenume", "emailul@gmail.com", "+40712345678", "STR.Test1", "Password1")]
        [TestCase("Nume2", "Prenume2", "emailul2@gmail.com", "+40712345678", "STR.Test2", "Password2")]
        public void RegisterValidUser(string lastName, string firstName, string email, string phoneNumber, string address, string password)
        {
            Account newAccount = new(lastName, firstName, email, phoneNumber, address, password);
            mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            RegisterWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(newAccount).Result;

            Assert.IsTrue(response.Success);
            Assert.That(mockDbContext.Object.Accounts.Count(), Is.EqualTo(1));
        }

        [Test]
        [TestCase("Vahid", "Hossein", "husse@gmail", "+40712345678", "STR.Test", "Password")]
        [TestCase("Nume", "Prenume", "invalid1@.com", "+40712345678", "STR.Test1", "Password1")]
        [TestCase("Nume2", "Prenume2", "invalid2", "+40712345678", "STR.Test2", "Password2")]
        [TestCase("Nume2", "Prenume2", "invalid..3@gmail.com", "+40712345678", "STR.Test3", "Password2")]
        [TestCase("Nume2", "Prenume2", "invalid._4@gmail.com", "+40712345678", "STR.Test4", "Password2")]
        [TestCase("Nume2", "Prenume2", "invalid__5@gmail.com", "+40712345678", "STR.Test5", "Password2")]
        public void TryToRegisterInvalidEmail(string lastName, string firstName, string email, string phoneNumber, string address, string password)
        {
            Account newAccount = new(lastName, firstName, email, phoneNumber, address, password);
            mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(0);

            RegisterWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(newAccount).Result;

            Assert.IsFalse(response.Success);
            Assert.That(response.Message, Is.EqualTo("Invalid email"));
            Assert.That(mockDbContext.Object.Accounts.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCase("Vahid", "Hossein", "husse@gmail.com", "++40712345678", "STR.Test", "Password")]
        [TestCase("Nume", "Prenume", "invalid1@gmail.com", "+407h12345678", "STR.Test1", "Password1")]
        [TestCase("Nume2", "Prenume2", "invalid2@gmail.com", "+407123456!78", "STR.Test2", "Password2")]
        public void TryToRegisterInvalidPhoneNumber(string lastName, string firstName, string email, string phoneNumber, string address, string password)
        {
            Account newAccount = new(lastName, firstName, email, phoneNumber, address, password);
            mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(0);

            RegisterWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(newAccount).Result;

            Assert.IsFalse(response.Success);
            Assert.That(response.Message, Is.EqualTo("Invalid phone number"));
            Assert.That(mockDbContext.Object.Accounts.Count(), Is.EqualTo(0));
        }

        [Test]
        [TestCase("Vahid", "Hossein", "husse@gmail.com", "+40712345678", "STR.Test", "Password")]
        [TestCase("Nume", "Prenume", "emailul@gmail.com", "+40712345678", "STR.Test1", "Password1")]
        [TestCase("Nume2", "Prenume2", "emailul2@gmail.com", "+40712345678", "STR.Test2", "Password2")]
        public void FailToSaveIntoDatabase(string lastName, string firstName, string email, string phoneNumber, string address, string password)
        {
            Account newAccount = new(lastName, firstName, email, phoneNumber, address, password);
            mockDbContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(0);

            RegisterWorkflow workflow = new(mockDbContext.Object);
            GeneralWorkflowResponse response = workflow.Execute(newAccount).Result;

            Assert.IsFalse(response.Success);
            Assert.That(response.Message, Is.EqualTo("Something went wrong"));
        }
    }
}

