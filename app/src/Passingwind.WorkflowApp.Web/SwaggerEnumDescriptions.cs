using System;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Passingwind.WorkflowApp.Web;

public class SwaggerEnumDescriptions : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (type.IsEnum)
        {
            var names = Enum.GetNames(type);

            var values2 = new OpenApiArray();

            values2.AddRange(names.Select(x => new OpenApiObject
            {
                ["name"] = new OpenApiString(Convert.ToInt32(Enum.Parse(type, x)).ToString()),
                ["value"] = new OpenApiString(x),
            }));

            var values1 = new OpenApiArray();
            values1.AddRange(names.Select(x => new OpenApiString(x)));

            schema.Extensions.Add(
                "x-enumNames",
                values1
            );

            schema.Extensions.Add(
                "x-ms-enum",
                new OpenApiObject
                {
                    ["name"] = new OpenApiString(type.Name),
                    ["modelAsString"] = new OpenApiBoolean(true),
                    ["values"] = values2,
                }
            );
        }
    }
}
