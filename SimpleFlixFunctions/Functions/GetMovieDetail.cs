using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SimpleFlixFunctions.Models;
using System.Net;

namespace SimpleFlixFunctions.Functions;

public class GetMovieDetail(ILogger<GetMovieDetail> logger, CosmosClient cosmosClient)
{
    [Function("GetMovieDetail")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        logger.LogInformation("Obtendo os dados");

        var container = cosmosClient.GetContainer("SimpleFlix", "movies");
        var id = req.Query["id"];
        var query = $"SELECT * FROM c WHERE c.id = @id";
        var queryDefinition = new QueryDefinition(query).WithParameter("@id", id);
        var result = container.GetItemQueryIterator<MovieResponse>(queryDefinition);

        var list = new List<MovieResponse>();

        while (result.HasMoreResults)
        {
            foreach (var item in await result.ReadNextAsync())
            {
                list.Add(item);
            }
        }

        var responseMessage = req.CreateResponse(HttpStatusCode.OK);
        await responseMessage.WriteAsJsonAsync(list.FirstOrDefault());

        return responseMessage;
    }
}

