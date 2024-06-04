using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPIDemo.Filters.OperationFilter
{
    public class AuthorizationHeaderOperationFilter : IOperationFilter
    {
        // our purpose of implementing this method is just to add a requirement that requires the authorization filter for each endpoint that swagger tries to document.
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if(operation.Security == null)
            {
                operation.Security = new List<OpenApiSecurityRequirement>();
            }

            // this scheme will be used as a security requirement
            var scheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } };
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [scheme] = new List<string>()
            });
        }
    }
}
