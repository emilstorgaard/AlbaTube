using AlbaTube.Dtos.Request;
using AlbaTube.Helpers;
using AlbaTube.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlbaTube.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAll();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("authorized")]
    public async Task<IActionResult> Get()
    {
        int userId = UserHelper.GetUserId(User);

        var result = await _userService.GetUser(userId);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> AddUser([FromForm] UserReqDto userReqDto)
    {
        await _userService.AddUser(userReqDto);
        return Ok("User registered successfully.");
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        int userId = UserHelper.GetUserId(User);

        await _userService.Delete(userId);
        return Ok("User was successfully deleted");
    }

    [Authorize]
    [HttpPost("{creatorId}/subscribe")]
    public async Task<IActionResult> Subscribe(int creatorId)
    {
        int subscriberId = UserHelper.GetUserId(User);

        if (subscriberId == 0) return Unauthorized("User not logged in.");

        await _userService.Subscribe(subscriberId, creatorId);

        return Ok(new { Message = "Subscribed successfully" });
    }

    [Authorize]
    [HttpDelete("{creatorId}/subscribe")]
    public async Task<IActionResult> Unsubscribe(int creatorId)
    {
        int subscriberId = UserHelper.GetUserId(User);

        if (subscriberId == 0) return Unauthorized("User not logged in.");

        await _userService.Unsubscribe(subscriberId, creatorId);

        return Ok(new { Message = "Unsubscribed successfully" });
    }
}
