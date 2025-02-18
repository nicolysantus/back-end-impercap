using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using System.Linq;

public class FormFileOperationFilter : IOperationFilter

{

    public void Apply(OpenApiOperation operation, OperationFilterContext context)

    {

        // Adiciona parâmetros para arquivos IFormFile

        var formFiles = context.MethodInfo.GetParameters()

            .Where(p => p.ParameterType == typeof(IFormFile) || p.ParameterType == typeof(IFormFile[]));

        foreach (var file in formFiles)

        {

            operation.Parameters.Add(new OpenApiParameter

            {

                Name = file.Name,

                In = ParameterLocation.Query, // Alterado para Query, pois o Swagger não suporta diretamente o Form

                Required = true,

                Schema = new OpenApiSchema

                {

                    Type = "string",

                    Format = "binary"

                }

            });

        }

        // Adiciona um parâmetro "Content-Type" para multipart/form-data

        if (operation.Parameters.All(p => p.Name != "Content-Type"))

        {

            operation.Parameters.Add(new OpenApiParameter

            {

                Name = "Content-Type",

                In = ParameterLocation.Header,

                Required = true,

                Schema = new OpenApiSchema { Type = "string" }

            });

        }

    }

}
