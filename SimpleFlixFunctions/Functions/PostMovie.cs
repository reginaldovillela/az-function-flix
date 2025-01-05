using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleFlixFunctions.Models;

namespace SimpleFlixFunctions.Functions;

public class PostMovie(ILogger<PostMovie> logger)
{
    [Function("PostMovie")]
    [CosmosDBOutput("%CosmoDBName%", 
                    "%ContainerName%", 
                    Connection = "CosmoDBConnection", 
                    CreateIfNotExists = true, 
                    PartitionKey = "id")]
    public async Task<object?> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        var content =  await new StreamReader(req.Body).ReadToEndAsync();

        MovieRequest movie = null;

        try
        {
            movie = JsonConvert.DeserializeObject<MovieRequest>(content);
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult($"Error ao deserializar o objeto: {ex.Message}");
        }

        return JsonConvert.DeserializeObject<MovieRequest>(content);
    }
}

