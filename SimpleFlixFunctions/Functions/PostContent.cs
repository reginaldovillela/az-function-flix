using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SimpleFlixFunctions.Functions;

public class PostContent(ILogger<PostContent> logger)
{
    [Function("PostContent")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("Processando a imagem no storage");

        if (!req.Headers.TryGetValue("file-type", out var fileTypeHeader))
        {
            return new BadRequestObjectResult("O cabeçalho 'file-type' é obrigatório.");
        }

        var fileType = fileTypeHeader.FirstOrDefault();
        var form = await req.ReadFormAsync();
        var file = form.Files["file"];

        if (file == null || file.Length == 0)
        {
            return new BadRequestObjectResult("O arquivo não foi enviado.");
        }

        var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        var containerName = fileType;

        var blobContainerClient = new BlobContainerClient(connectionString, containerName);

        await blobContainerClient.CreateIfNotExistsAsync();
        await blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

        var blobName = file.FileName;
        var blob = blobContainerClient.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            await blob.UploadAsync(stream, true);
        }

        logger.LogInformation("Arquivo {FileName} salvo com sucesso no container {ContainerName}.", file.FileName, containerName);

        return new OkObjectResult(new
        {
            Message = "Arquivo salvo com sucesso no storage",
            BlobUri = blob.Uri
        });
    }
}
