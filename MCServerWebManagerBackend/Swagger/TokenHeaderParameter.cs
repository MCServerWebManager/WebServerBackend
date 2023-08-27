using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Net.Http.Headers;

namespace MCServerWebManagerBackend.Swagger;

public class TokenHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        
        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = "X-User-Token",
            In = ParameterLocation.Header,
            Description = "User Token"
        });
    }
}