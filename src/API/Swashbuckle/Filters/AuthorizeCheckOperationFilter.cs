using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System;
using System.Linq;
using System.Reflection;

namespace api.Swashbuckle.Filters
{
    /// <summary>
    /// Adds 401/403 responses to endpoints with Authorize attribute applied.
    /// </summary>
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            context.ApiDescription.TryGetMethodInfo(out var methodInfo);

            if (methodInfo == null)
            {
                return;
            }

            var hasAuthorizeAttribute = false;

            if (methodInfo.MemberType == MemberTypes.Method)
            {
                // NOTE: Check the controller itself has Authorize attribute
                hasAuthorizeAttribute = methodInfo.DeclaringType
                                                  .GetCustomAttributes(true)
                                                  .OfType<AuthorizeAttribute>()
                                                  .Any();

                // NOTE: Controller has Authorize attribute, so check the endpoint itself.
                //       Take into account the allow anonymous attribute
                if (hasAuthorizeAttribute)
                {
                    hasAuthorizeAttribute = !methodInfo.GetCustomAttributes(true)
                                                       .OfType<AllowAnonymousAttribute>()
                                                       .Any();
                }
                else
                {
                    hasAuthorizeAttribute = methodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();
                }
            }

            if (!hasAuthorizeAttribute)
            {
                return;
            }

            if (!operation.Responses.Keys.Contains(StatusCodes.Status401Unauthorized.ToString()))
            {
                operation.Responses
                         .Add(StatusCodes.Status401Unauthorized.ToString(),
                              new OpenApiResponse
                              { Description = "Unauthorized" });
            }

            if (!operation.Responses.Keys.Contains(StatusCodes.Status403Forbidden.ToString()))
            {
                operation.Responses
                         .Add(StatusCodes.Status403Forbidden.ToString(),
                              new OpenApiResponse
                              { Description = "Forbidden" });
            }
        }
    }
}
