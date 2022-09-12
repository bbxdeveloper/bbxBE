using bbxBE.Domain.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
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
/*
        public static string GenerateJSONWebToken(Users user, AxegazMobileSrvConfig p_config)
        {
            ShortGuid tokenID = ShortGuid.NewGuid();
            List<Claim> claims = new List<Claim>()
            {
                new Claim( ClaimTypes.Thumbprint, tokenID.Value),
                new Claim( ClaimTypes.Name, p_user.FirstName + " " + p_user.LastName),
                new Claim( ClaimTypes.NameIdentifier, p_user.ID.ToString())
          };
            return GenerateJSONWebToken(tokenID, claims, p_config);
        }


        //https://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
        public static string GenerateJSONWebToken(ShortGuid p_tokenID, List<Claim> p_claims, AxegazMobileSrvConfig p_config)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(p_config.JWTKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var encryptingCredentials = new EncryptingCredentials(key, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);

            IdentityModelEventSource.ShowPII = true;

            var jwtSecurityToken = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
                p_config.JWTIssuer,
                p_config.JWTIssuer,       //Issuer=Audience..
                new ClaimsIdentity(p_claims),
                null,
                DateTime.UtcNow.AddMinutes(p_config.JWTTokenExpire),
                null,
                signingCredentials: creds,
                encryptingCredentials: encryptingCredentials
                );
            var encryptedJWT = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            var jwtsess = new JWTSession() { ID = p_tokenID, Token = encryptedJWT, Timestamp = DateTime.UtcNow };
            JWTSessionModelCache.Instance.Items.Add(jwtsess);

            return encryptedJWT;
        }
*/
    }
}
