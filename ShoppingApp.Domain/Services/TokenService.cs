using System;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ShoppingApp.Domain.Models;
using LanguageExt;

namespace ShoppingApp.Domain.Services
{
    public static class TokenService
    {
        public static string CreateToken(Account account)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, account.Uid.ToString())
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

        public static Option<string> ExtractAccountID(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Claim? accountID = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            if (accountID != null)
            {
                return Option<string>.Some(accountID.Value);
            }

            return Option<string>.None;
        }
    }
}

