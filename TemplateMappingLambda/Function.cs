using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using RepFabric.Api.BL.Interfaces;
using RepFabric.Api.Models.DynamoDb;
using RepFabric.Api.Models.Request;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TemplateMappingLambda;

public class Function
{
    private readonly IDynamoDbTemplateMappingService _mappingService;

    public Function()
    {
        var services = new ServiceCollection();
        // Register your services and configuration here
        // services.AddScoped<IDynamoDbTemplateMappingService, DynamoDbTemplateMappingService>();
        _mappingService = services.BuildServiceProvider().GetRequiredService<IDynamoDbTemplateMappingService>();
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper.
    ///
    /// To use this handler to respond to an AWS event, reference the appropriate package from 
    /// https://github.com/aws/aws-lambda-dotnet#events
    /// and change the string input parameter to the desired event type. When the event type
    /// is changed, the handler type registered in the main method needs to be updated and the LambdaFunctionJsonSerializerContext 
    /// defined below will need the JsonSerializable updated. If the return type and event type are different then the 
    /// LambdaFunctionJsonSerializerContext must have two JsonSerializable attributes, one for each type.
    ///
    // When using Native AOT extra testing with the deployed Lambda functions is required to ensure
    // the libraries used in the Lambda function work correctly with Native AOT. If a runtime 
    // error occurs about missing types or methods the most likely solution will be to remove references to trim-unsafe 
    // code or configure trimming options. This sample defaults to partial TrimMode because currently the AWS 
    // SDK for .NET does not support trimming. This will result in a larger executable size, and still does not 
    // guarantee runtime trimming errors won't be hit. 
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>

    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        try
        {
            var path = request.Path?.ToLower() ?? "";
            var method = request.HttpMethod?.ToUpperInvariant() ?? "";

            // CREATE: POST /mappings
            if (method == "POST" && path.EndsWith("/mappings"))
            {
                var mappingRequest = JsonSerializer.Deserialize<ExcelMappingRequest>(request.Body);
                if (mappingRequest == null)
                    return BadRequest("Invalid request body.");

                var mappingJson = JsonSerializer.Serialize(mappingRequest.Mappings);
                await _mappingService.CreateAsync(mappingRequest.TemplateFileName, mappingJson);
                return Ok("Mapping saved successfully.");
            }

            // READ ALL: GET /mappings
            if (method == "GET" && path.EndsWith("/mappings"))
            {
                var mappings = await _mappingService.GetAllAsync();
                return Ok(mappings);
            }

            // UPDATE: PUT /mapping/{id}
            if (method == "PUT" && path.Contains("/mapping/"))
            {
                if (!request.PathParameters.TryGetValue("id", out var idStr) || !int.TryParse(idStr, out var id))
                    return BadRequest("Invalid or missing id.");

                var mapping = JsonSerializer.Deserialize<TemplateMapping>(request.Body);
                if (mapping == null || string.IsNullOrWhiteSpace(mapping.TemplateFileName) || string.IsNullOrWhiteSpace(mapping.MappingJson))
                    return BadRequest("TemplateFileName and MappingJson are required.");

                await _mappingService.UpdateAsync(id, mapping);
                return Ok("Mapping updated successfully.");
            }

            // DELETE: DELETE /mapping/{id}
            if (method == "DELETE" && path.Contains("/mapping/"))
            {
                if (!request.PathParameters.TryGetValue("id", out var idStr) || !int.TryParse(idStr, out var id))
                    return BadRequest("Invalid or missing id.");

                await _mappingService.DeleteAsync(id);
                return Ok("Mapping deleted successfully.");
            }

            return NotFound("Endpoint not found.");
        }
        catch (Exception ex)
        {
            context.Logger.LogError(ex, "Error processing mapping request.");
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = "Internal server error."
            };
        }
    }

    // Helper methods for responses
    private APIGatewayProxyResponse Ok(object result) => new()
    {
        StatusCode = 200,
        Body = JsonSerializer.Serialize(result),
        Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
    };

    private APIGatewayProxyResponse BadRequest(string message) => new()
    {
        StatusCode = 400,
        Body = message
    };

    private APIGatewayProxyResponse NotFound(string message) => new()
    {
        StatusCode = 404,
        Body = message
    };
}

/// <summary>
/// This class is used to register the input event and return type for the FunctionHandler method with the System.Text.Json source generator.
/// There must be a JsonSerializable attribute for each type used as the input and return type or a runtime error will occur 
/// from the JSON serializer unable to find the serialization information for unknown types.
/// </summary>
[JsonSerializable(typeof(string))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
    // By using this partial class derived from JsonSerializerContext, we can generate reflection free JSON Serializer code at compile time
    // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
    // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}