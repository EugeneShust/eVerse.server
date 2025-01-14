using API.Server.Repository;
using Microsoft.AspNetCore.Mvc;
using Shared.Protocol.Requests;
using Internal.Extentions;
using MongoDB.Driver.GridFS;
using AutoMapper;
using Shared.Protocol.Dtos;
using API.Server.Models;
using Microsoft.Extensions.Logging;

namespace api.server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserRepository userRepository, IGridFSBucket gridFs, IVerseRepository verseRepository, IMapper mapper) : ControllerBase
    {
        private readonly IUserRepository _users = userRepository;
        private readonly IVerseRepository _verses = verseRepository;
        private readonly IGridFSBucket _gridFs = gridFs;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [Route("profile")]
        public async Task<ActionResult<ProfileResponse>> Profile([FromBody] ProfileRequest request)
        {
            var userId = HttpContext.GetUserId();
            var user = await _users.FindOneAsync(x => x.Id == userId);

            var response = _mapper.Map<ProfileResponse>(user);

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

            var verses = await _verses.FindManyAsync(model => user.Verses.Contains(model.Id));
            var favorites = await _verses.FindManyAsync(model => user.Favorites.Select(favorite => favorite.Id).Contains(model.Id));

            var response = new UserVersesResponse
            {
                Verses = verses.Select(x => _mapper.Map<VersePreviewDto>(x)).ToList(),
                Favorites = favorites.Select(x => _mapper.Map<VersePreviewDto>(x)).ToList()
            };

            return Ok(response);
        }

        [HttpPost("favorite/{id}")]
        public async Task<IActionResult> ToggleVerseFavorite(string id)
        {
            var userId = HttpContext.GetUserId();
            var user = await _users.FindOneAsync(x => x.Id == userId);
            var verse = await _verses.FindOneAsync(x => x.Id == id);

            var favorite = user.Favorites.FirstOrDefault(x => x.Id == id);

            if (favorite == null)
            {
                user.Favorites.Add(new Favorite { Id = id });
                verse.Participants.Add(new Participant { Id = userId });
            }
            else
            {
                favorite.IsActive = !favorite.IsActive;
                var participant = verse.Participants.FirstOrDefault(x => x.Id == userId);
                if (participant == null)
                {
                    participant = new Participant { Id = userId };
                }
                else
                {
                    participant.IsActive = !participant.IsActive;
                }
            }

            await _users.ReplaceOneAsync(x => x.Id == user.Id, user);
            await _verses.ReplaceOneAsync(x => x.Id == verse.Id, verse);

            return Ok();
        }

        [HttpPost("favorite/{id}/{eventId}")]
        public async Task<IActionResult> ToggleVerseEventFavorite(string id, string eventId)
        {
            var userId = HttpContext.GetUserId();
            var user = await _users.FindOneAsync(x => x.Id == userId);
            var verse = await _verses.FindOneAsync(x => x.Id == id);

            var favorite = user.Favorites.FirstOrDefault(x => x.Id == id);

            if (favorite == null)
            {
                favorite = new Favorite { Id = id };
                favorite.Events.Add(eventId);
                user.Favorites.Add(favorite);

                verse.Participants.Add(new Participant { Id = userId });
            }
            else
            {
                if (!favorite.Events.Remove(eventId))
                    favorite.Events.Add(eventId);
            }

            await _users.ReplaceOneAsync(x => x.Id == user.Id, user);

            return Ok();
        }
    }
}