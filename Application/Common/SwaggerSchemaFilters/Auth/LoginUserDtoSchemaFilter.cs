using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Application.Common.SwaggerSchemaFilters.Auth;

public class LoginUserDtoSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = new OpenApiObject
        {
            ["DeviceId"] = new OpenApiString("643534gfg4353"),
            ["GameName"] = new OpenApiString("Crewmates")
        };
    }
}