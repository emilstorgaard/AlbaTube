using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AlbaTube.Dtos.Request;
using AlbaTube.Helpers;
using AlbaTube.Services.Interfaces;
using AlbaTube.Dtos.Response;
using AlbaTube.Repositories.Interfaces;

namespace AlbaTube.Controllers;

[Route("api/videos")]
[ApiController]
public class VideosController : ControllerBase
{
    private readonly IVideoService _videoService;
    private readonly IVideoRepository _videoRepository;

    public VideosController(IVideoService videoService, IVideoRepository videoRepository)
    {
        _videoService = videoService;
        _videoRepository = videoRepository;
    }

    [HttpGet("{id:int}/stream")]
    public async Task<IActionResult> StreamVideo(int id)
    {
        var isRangeRequest = Request.Headers.ContainsKey("Range");

        if (!isRangeRequest)
        {
            await _videoRepository.IncrementViewCount(id);
        }

        var streamResult = await _videoService.Stream(id);
        return File(streamResult, "video/mp4", enableRangeProcessing: true);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VideoRespDto>> GetVideo(int id)
    {
        var result = await _videoService.GetVideoById(id);
        return Ok(result);
    }

    [HttpGet("user/{id}")]
    public async Task<ActionResult<List<VideoRespDto>>> GetAllVideosByUserId(int id)
    {
        int userId = UserHelper.GetUserId(User);

        var result = await _videoService.GetAllVideosByUserId(id);
        return Ok(result);
    }

    [HttpGet("thumbnail/{*imagePath}")]
    public IActionResult GetThumbnailImage(string imagePath)
    {
        var thumbnailPath = _videoService.GetThumbnail(imagePath);
        return PhysicalFile(thumbnailPath, "image/jpeg");
    }

    [Authorize]
    [HttpPost]
    [RequestSizeLimit(1_000_000_000)] // 1 GB
    public async Task<IActionResult> UploadVideo([FromForm] VideoReqDto videoDto)
    {
        int userId = UserHelper.GetUserId(User);

        await _videoService.Upload(videoDto, userId);
        return StatusCode(201, "Video was successfully uploaded.");
    }

    [Authorize]
    [HttpPost("{id:int}/like")]
    public async Task<IActionResult> LikeVideo(int id)
    {
        int userId = UserHelper.GetUserId(User);

        await _videoService.Like(id, userId);
        return Ok("Video was successfully liked.");
    }

    [Authorize]
    [HttpPost("{id:int}/dislike")]
    public async Task<IActionResult> DislikeVideo(int id)
    {
        int userId = UserHelper.GetUserId(User);

        await _videoService.Dislike(id, userId);
        return Ok("Video was successfully disliked.");
    }

    [Authorize]
    [HttpDelete("{id:int}/thumbnail/remove")]
    public async Task<IActionResult> RemoveThumbnail(int id)
    {
        int userId = UserHelper.GetUserId(User);

        await _videoService.UpdateThumbnail(id, userId);
        return Ok("Video thumbnail was successfully removed.");
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateVideo(int id, [FromForm] VideoReqDto videoDto)
    {
        int userId = UserHelper.GetUserId(User);

        await _videoService.Update(id, videoDto, userId);
        return Ok("Video was successfully updated.");
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteVideo(int id)
    {
        int userId = UserHelper.GetUserId(User);

        await _videoService.Delete(id, userId);
        return Ok("Video was successfully deleted.");
    }
}
