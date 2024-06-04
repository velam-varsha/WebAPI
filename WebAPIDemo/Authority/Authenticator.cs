using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPIDemo.Authority
{
    public static class Authenticator
    {
        public static bool Authenticate(string clientId, string secret)
        {
            var app = AppRepository.GetApplicationByClientId(clientId);
            if (app == null) return false;

            return (app.ClientId == clientId && app.Secret == secret);
        }

        public static string CreateToken(string clientId, DateTime expiresAt, string strSecretKey)
        {
            // Algorithm
            // Payload (claims)
            // Signing key

            var app = AppRepository.GetApplicationByClientId(clientId);

            var claims = new List<Claim>
            {
                new Claim("AppName", app?.ApplicationName??string.Empty ),

                // we added delete scope in the last, so if we want to include delete in here like read and write, then we should code in many places wch isnt good. so we will do the implementation separately below
                //new Claim("Read", (app?.Scopes??string.Empty).Contains("read")?"true":"false"),
                //new Claim("Write", (app?.Scopes??string.Empty).Contains("write")?"true":"false"),
            };

            var scopes = app?.Scopes?.Split(",");
            
            if(scopes != null && scopes.Length > 0)
            {
                // this will allow us to automatically generate the claims based on what is registered in  WebAPIDemo.Authority.AppRepository file
                foreach (var scope in scopes)
                {
                    claims.Add(new Claim(scope.ToLower(), "true")); 
                }
            }

            var secretKey = Encoding.ASCII.GetBytes(strSecretKey);

            var jwt = new JwtSecurityToken(
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature),
                claims: claims,
                expires: expiresAt,
                notBefore: DateTime.UtcNow
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);   // Serializes the JWT into a string and returns it.
        }

        public static /* bool */ IEnumerable<Claim>? VerifyToken(string token, string strSecretKey)
        {
            if (string.IsNullOrWhiteSpace(token)) { return /* false */ null; }

            if (token.StartsWith("Bearer"))
            {
                token = token.Substring(6).Trim();
            }

            var secretKey = Encoding.ASCII.GetBytes(strSecretKey);  // Converts the secret key string into a byte array.
            SecurityToken securityToken;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,    // Validates the signing key.
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),   // Specifies the symmetric security key.
                    ValidateLifetime = true,    // Validates the token's lifetime.
                    ValidateAudience = false,   // Does not validate the audience
                    ValidateIssuer = false,  //Does not validate the issuer.
                    ClockSkew = TimeSpan.Zero // Sets clock skew to zero.
                },
                out securityToken);    // Validates the token using the specified validation parameters and outputs the result to securityToken

                // we can see that we are using tokenHandler to verify the token And coz we are using tokenHandler here, tokenHandler is able to actually look into the token so we can use this place to actually get the claims and return that claims from the verifyToken method
                if (securityToken != null)
                {
                    var tokenObject = tokenHandler.ReadJwtToken(token);
                    // securityToken data type is SecurityToken and tokenObject data type is JwtSecurityToken, here both the data types are different form each other. And the reason why we need this JWT security token is that this JwtSecurityToken object here contains claims
                    return tokenObject.Claims??(new List<Claim>());  // So here we are going to return Claims, But if the Claims is null then we are going to return a empty list of Claims. ??: 1st question mark indicates null, 2nd question mark indicates ternary operator
                    // in above line tokenObject is not null, if tokenObject.Claims is null we are returning an empty list, but here verification succeeded just that we dont have any claims so we return an empty list. 
                }
                // so above if block, if the securityToken is not null then we return claims. if securityToken is null then we return null in else block.
                else
                {
                    return null;  // here securityToken is null means verification failed
                }
            }
            catch (SecurityTokenException)
            {
                return /* false */ null;
            }
            catch
            {
                throw;
            }
            // the below line we will implement in the try block
           // return securityToken != null;
        }
    }
}
