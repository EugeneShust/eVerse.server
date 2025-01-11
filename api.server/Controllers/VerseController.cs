using API.Server.Models;
using API.Server.Repository;
using AutoMapper;
using Internal.Extentions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Shared.Protocol.Dtos;
using Shared.Protocol.Requests;

namespace API.Server.Controllers
{
    [Route("api/[controller]")]
    public class VerseController(IUserRepository userRepository, IGridFSBucket gridFs, IVerseRepository verseRepository, IMapper mapper) : ControllerBase
    {
        private readonly IUserRepository _users = userRepository;
        private readonly IVerseRepository _verseRepository = verseRepository;
        private readonly IGridFSBucket _gridFs = gridFs;
        private readonly IMapper _mapper = mapper;

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<VerseCreateResponse>> Create([FromBody] VerseCreateRequest request)
        {
            var userId = HttpContext.GetUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var newVerse = new Verse { AuthorId = userId, Logo = request.Logo, Name = request.Name, Start = request.Start, End = request.End };

            await _verseRepository.CreateAsync(newVerse);


            var update = Builders<User>.Update.AddToSet(user => user.Verses, newVerse.Id);

            // Виконуємо оновлення
            var result = await _users.UpdateOneAsync(x=>x.Id == userId, update);


            var response = new VerseCreateResponse
            {
                VerseId = newVerse.Id
            };

            return Ok(response);
        }
    }
}
