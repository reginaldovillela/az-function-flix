using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SimpleFlixFunctions.Models;
using System.Net;

namespace SimpleFlixFunctions.Functions;

public class GetMoviesAll(ILogger<GetMoviesAll> logger, CosmosClient cosmosClient)
{
    [Function("GetMoviesAll")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        logger.LogInformation("Obtendo os dados");

        var container = cosmosClient.GetContainer("SimpleFlix", "movies");
        var query = $"SELECT * FROM c";
        var queryDefinition = new QueryDefinition(query);
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
        await responseMessage.WriteAsJsonAsync(list);

        return responseMessage;
    }
}

