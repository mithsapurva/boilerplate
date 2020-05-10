

namespace Authentication.API
{ 
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Common;

    public class TokenBuilder : ITokenBuilder
    {
        public string BuildToken(string systemName)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;
            //set the time when it expires
            DateTime expires = DateTime.UtcNow.AddMinutes(Constants.TokenExpiryDuration);

            var tokenHandler = new JwtSecurityTokenHandler();
            //create a identity and add claims to the user which we want to log in
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, systemName)
            });

            var signingSecurityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(Constants.TokenSigningKey));
            var encryptionSecurityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(Constants.TokenEncryptionKey));

            var signingCredentials = new SigningCredentials(signingSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            //encrypt the key
            var ep = new EncryptingCredentials(encryptionSecurityKey, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);
            //create the jwt
            var token = tokenHandler.CreateJwtSecurityToken(issuer: null,
                                                                            audience: null,
                                                                            subject: claimsIdentity,
                                                                            notBefore: issuedAt,
                                                                            expires: expires,
                                                                            issuedAt: issuedAt,
                                                                            signingCredentials: signingCredentials,
                                                                            encryptingCredentials: ep);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}
