using API.Server.Repository;
using Microsoft.AspNetCore.Mvc;
using Shared.Protocol.Requests;
using Internal.Extentions;
using MongoDB.Driver.GridFS;
using AutoMapper;
using Shared.Protocol.Dtos;

namespace api.server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserRepository userRepository, IGridFSBucket gridFs, IVerseRepository verseRepository, IMapper mapper) : ControllerBase
    {
        private readonly IUserRepository _users = userRepository;
        private readonly IVerseRepository _verseRepository = verseRepository;
        private readonly IGridFSBucket _gridFs = gridFs;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [Route("profile")]
        public async Task<ActionResult<ProfileResponse>> Profile([FromBody] ProfileRequest request)
        {
            var userId = HttpContext.GetUserId();
            var user = await _users.FindOneAsync(x => x.Id == userId);

            var response = new ProfileResponse
            {
                Profile = _mapper.Map<ProfileDto>(user)
            };

            return Ok(response);
        }

        [HttpPost("profile/avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            var userId = HttpContext.GetUserId();

            using var stream = file.OpenReadStream();

            var fileId = await _gridFs.UploadFromStreamAsync($"{userId}_avatar", stream, new GridFSUploadOptions
            {
                Metadata = new MongoDB.Bson.BsonDocument
                {
                    { "userId", userId },
                    { "fileName", file.FileName },
                    { "contentType", file.ContentType }
                }
            });

            return Ok(new { FileId = fileId.ToString() });
        }

        [HttpGet("profile/avatar")]
        public async Task<IActionResult> GetAvatar(string userId)
        {
            try
            {
                var fileName = $"{userId}_avatar";
                var fileStream = await _gridFs.OpenDownloadStreamByNameAsync(fileName);

                return File(fileStream, fileStream.FileInfo.Metadata["contentType"].AsString);
            }
            catch (GridFSFileNotFoundException)
            {
                return NotFound("Avatar not found");
            }
        }

        [HttpPost("verses")]
        public async Task<IActionResult> GetVerses()
        {
            var userId = HttpContext.GetUserId();
            var user = await _users.FindOneAsync(x => x.Id == userId);

            var verses = await _verseRepository.FindManyAsync(model => user.Verses.Contains(model.Id));
            var favorites = await _verseRepository.FindManyAsync(model => user.Favorites.Contains(model.Id));

            var response = new UserVersesResponse
            {
                Verses = verses.Select(x => _mapper.Map<VerseDto>(x)).ToList(),
                Favorites = favorites.Select(x=>_mapper.Map<VerseDto>(x)).ToList()
            };

            return Ok(response);
        }
    }
}