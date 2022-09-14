using bbxBE.Common;
using bbxBE.Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace bbxBE.Application.BLL
{
    public static class BllAuth
    {

        public static string GetPwdHash(string pwd, string salt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: pwd,
                salt: Encoding.ASCII.GetBytes(salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public static string GenerateJSONWebToken(Users user, string JWTKey, string JWTIssuer, double JWTTokenExpire)
        {
            ShortGuid tokenID = ShortGuid.NewGuid();
            List<Claim> claims = new List<Claim>()
            {
                new Claim( ClaimTypes.Thumbprint, tokenID.Value),
                new Claim( ClaimTypes.Name, user.Name),
                new Claim( ClaimTypes.NameIdentifier, user.ID.ToString())
          };
            return GenerateJSONWebToken(tokenID, claims,  JWTKey,  JWTIssuer,  JWTTokenExpire);
        }


        //https://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
        public static string GenerateJSONWebToken(ShortGuid p_tokenID, List<Claim> p_claims, string JWTKey, string JWTIssuer, double JWTTokenExpire)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var encryptingCredentials = new EncryptingCredentials(key, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);

            IdentityModelEventSource.ShowPII = true;

            var jwtSecurityToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
                JWTIssuer,
                JWTIssuer,       //Issuer=Audience..
                new ClaimsIdentity(p_claims),
                null,
                DateTime.UtcNow.AddMinutes(JWTTokenExpire),
                null,
                signingCredentials: creds,
                encryptingCredentials: encryptingCredentials
                );
            var encryptedJWT = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return encryptedJWT;
        }
    }
}
