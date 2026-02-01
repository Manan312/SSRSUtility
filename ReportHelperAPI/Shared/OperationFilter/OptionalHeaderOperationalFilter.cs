using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace Infrastructure.OperationFilter
{
    public class OptionalHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize =
            context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true
            || context.MethodInfo.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>().Any();

            if (!hasAuthorize)
                return;

            // Apply security requirement
            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

            operation.Parameters ??= new List<OpenApiParameter>();

            AddHeader(operation, "X-Client-Id", "Client identifier");
            AddHeader(operation, "X-Tenant-Id", "Tenant identifier");
        }

        private static void AddHeader(OpenApiOperation operation, string name, string description)
        {
            if (operation.Parameters.Any(p => p.Name == name))
                return;

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = name,
                In = ParameterLocation.Header,
                Required = false,
                Description = description,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
