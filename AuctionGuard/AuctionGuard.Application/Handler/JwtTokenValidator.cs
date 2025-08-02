using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionGuard.Application.Handler
{

    public static class JwtTokenValidator
    {
        public static bool ValidateToken(string token, string secretKey, string issuer,
         string audience, out JwtSecurityToken? decodedToken)
        {
            decodedToken = null;


            // Ensure none of the values are null or empty
            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT configuration is missing or invalid.");
            }

            // Set up token validation parameters
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                ValidateLifetime = true,  // Ensure token hasn't expired
                ClockSkew = TimeSpan.Zero // Optional: Reduces tolerance for token expiration
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validate token and retrieve principal
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                // Cast validated token to JwtSecurityToken and set output variable
                decodedToken = validatedToken as JwtSecurityToken;

                // Ensure that the token was generated using the expected algorithm
                if (decodedToken == null || decodedToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    return false;
                }

                // Token is valid and properly signed
                return true;
            }
            catch (Exception)
            {
                // Token validation failed
                return false;
            }
        }
    }
}
