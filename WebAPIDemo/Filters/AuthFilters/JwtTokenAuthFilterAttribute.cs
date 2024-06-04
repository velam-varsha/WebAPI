using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIDemo.Attributes;
using WebAPIDemo.Authority;

namespace WebAPIDemo.Filters.AuthFilters
{
    // When we implement IAsyncAuthorizationFilter, we can apply JWTTokenAuthFilterAttribute filter to the controller. So that means when the user tries to access any endpoint under that controller, it's going to hit this
    // JwtTokenAuthFilterAttribute filter before it actually executes the action method. And this filter is for the purpose of varifying the json web token in our case.
    public class JwtTokenAuthFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if(!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            // dependency injection of IConfiguration for getting secretKey
            var configuration = context.HttpContext.RequestServices.GetService<IConfiguration>();

            // we changed the code from return type bool to IEnumerable<Claim>? so we should alter the code here
            //if (!Authenticator.VerifyToken(token, configuration.GetValue<string>("SecretKey")))  // here we are verifying the token, now go to VerifyToken definition
            //{
            //    context.Result = new UnauthorizedResult();
            //}

            var claims = Authenticator.VerifyToken(token, configuration.GetValue<string>("SecretKey"));
            if(claims == null) // means verification failed
            {
                context.Result = new UnauthorizedResult();  // status code is 401
            }
            else
            {
                var requiredClaims = context.ActionDescriptor.EndpointMetadata.OfType<RequiredClaimAttribute>().ToList(); // if this failed then status code is 403
                
                if(requiredClaims != null &&  !requiredClaims.All(rc => claims.Any(c => c.Type.ToLower() == rc.ClaimType.ToLower() && c.Value.ToLower() == rc.ClaimValue.ToLower())))
                {
                    context.Result = new StatusCodeResult(403);
                }
                /* requiredClaims != null ensures that the requiredClaims list is not null.
                !requiredClaims.All(...) ensures that not all required claims are satisfied. All returns true only if every element in the requiredClaims list satisfies the given condition.
                The condition checks whether, for every required claim (rc), there exists at least one claim in the user's claims (claims) that matches both the claim type and claim value, ignoring case.
               
                Let's break down the condition inside All:

                rc => claims.Any(...) checks if there is any claim in the user's claims (claims) that satisfies the condition for each required claim (rc).
                c.Type.ToLower() == rc.ClaimType.ToLower() ensures that the claim type matches, ignoring case.
                c.Value.ToLower() == rc.ClaimValue.ToLower() ensures that the claim value matches, ignoring case.
                
                rc is a variable representing an individual required claim from the requiredClaims list.
                claims is a collection of claims associated with the current user.
                c is a variable representing an individual claim from the claims collection.
                 c is a variable that stands for an individual claim in the user's claims collection.
                The lambda expression c => c.Type.ToLower() == rc.ClaimType.ToLower() && c.Value.ToLower() == rc.ClaimValue.ToLower() checks if the claim type and value of c match the required claim's type and value, ignoring case.
                The Any method returns true if at least one claim in the user's claims matches the required claim type and value.
                */
            }
        }
    }
}
