using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFlixFunctions.Models;

internal class MovieResponse
{
    public string Id { get; init; }

    public string Title { get; init; }

    public string Year { get; init; }

    public string Video { get; init; }

    public string Thumbnail { get; init; }
}

