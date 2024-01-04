using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ShoppingApp.Data.Models;
using ShoppingApp.Domain.Models;

namespace ShoppingApp.Test
{
    public static class Utils
    {
        public static void CreateFakeAccounts(ref List<AccountDTO> accounts)
        {
            accounts.Add(new AccountDTO
            {
                Uid = Guid.NewGuid().ToString(),
                LastName = "Vahid",
                FirstName = "Hossein",
                Email = "husse@gmail.com",
                PhoneNumber = "+40712345678",
                Address = "STR. Test",
                Password = BCrypt.Net.BCrypt.HashPassword("1234")
            });

            accounts.Add(new AccountDTO
            {
                Uid = Guid.NewGuid().ToString(),
                LastName = "user2",
                FirstName = "user2",
                Email = "user2@gmail.com",
                PhoneNumber = "+40712345678",
                Address = "STR. Test2",
                Password = BCrypt.Net.BCrypt.HashPassword("1234")
            });

            accounts.Add(new AccountDTO
            {
                Uid = Guid.NewGuid().ToString(),
                LastName = "user3",
                FirstName = "user3",
                Email = "user3@gmail.com",
                PhoneNumber = "+40712345678",
                Address = "STR. Test3",
                Password = BCrypt.Net.BCrypt.HashPassword("1234")
            });
        }

        public static List<Product> CreateDefaultListOfThreeProducts() =>
            new List<Product>()
            {
                new("1", "1", "1", 1, 1),
                new("2", "2", "2", 1, 2),
                new("3", "3", "3", 1, 3),
            };
        public static List<Product> CreateDefaultListOfInexistentProducts() =>
            new List<Product>()
            {
                new("16", "16", "16", 1, 1),
                new("26", "26", "26", 1, 2),
                new("36", "36", "36", 1, 3),
            };

        public static void CreateFakeProducts(ref List<ProductDTO> products)
        {
            products.Add(new ProductDTO
            {
                Uid = "1",
                Name = "1",
                Description = "1",
                Quantity = 10,
                Price = 10
            });
            products.Add(new ProductDTO
            {
                Uid = "2",
                Name = "2",
                Description = "2",
                Quantity = 2,
                Price = 2
            });
            products.Add(new ProductDTO
            {
                Uid = "3",
                Name = "3",
                Description = "3",
                Quantity = 100,
                Price = 30
            });
            products.Add(new ProductDTO
            {
                Uid = "4",
                Name = "4",
                Description = "4",
                Quantity = 30,
                Price = 40
            });
        }
        public static string CreateFakeToken()
        {
            List<Claim> claims = new List<Claim> {
            
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRETKEY")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var issuer = "ShoppingApp";

            var token = new JwtSecurityToken(
                    issuer: issuer,
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }
    }
}

