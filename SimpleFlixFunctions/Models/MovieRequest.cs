namespace SimpleFlixFunctions.Models;

public class MovieRequest
{
    public string Id => Guid.NewGuid().ToString();

    public string Title { get; init; }

    public string Year { get; init; }

    public string Video { get; init; }

    public string Thumbnail { get; init; }
}
