using AlbaTube.Dtos.Response;
using AlbaTube.Helpers;
using AlbaTube.Repositories.Interfaces;
using AlbaTube.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AlbaTube.Controllers;

[Route("api/search")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }


    [HttpGet]
    public async Task<ActionResult<SearchRespDto>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Query cannot be empty");

        int loggedInUserId = UserHelper.GetUserId(User);
        var videos = await _searchService.Videos(q, loggedInUserId);
        var users = await _searchService.Users(q, loggedInUserId);

        var result = new SearchRespDto
        {
            Videos = videos,
            Users = users
        };

        return Ok(result);
    }
}
