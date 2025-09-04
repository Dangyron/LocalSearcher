using System.Diagnostics;
using LocalSearcher.Api.Common.Configs;
using LocalSearcher.Api.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LocalSearcher.Api.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController(ISearchService searchService, ILogger<SearchController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query parameter is required");
        
        var start = Stopwatch.GetTimestamp();
        logger.LogInformation("Search started for {Query}",  query);
        var results = await searchService.SearchAsync(new SearchOptions{Query = query}, cancellationToken);
        logger.LogInformation("Search ended for {Query}. Took {Time}",  query, Stopwatch.GetElapsedTime(start));
        return Ok(results);
    }
}