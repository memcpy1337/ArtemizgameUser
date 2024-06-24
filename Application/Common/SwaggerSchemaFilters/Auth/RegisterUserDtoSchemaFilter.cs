using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Application.Common.SwaggerSchemaFilters.Auth;

public class RegisterUserDtoSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = new OpenApiObject
        {
            ["DeviceId"] = new OpenApiString("643534gfg4353"),
            ["UserName"] = new OpenApiString("artemizgamef"),
            ["GameName"] = new OpenApiString("Crewmates"),
        };
    }
}