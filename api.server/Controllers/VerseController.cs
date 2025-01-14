using API.Server.Models;
using API.Server.Repository;
using AutoMapper;
using Internal.Extentions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Shared.Protocol.Dtos;
using Shared.Protocol.Requests;
using System;
using System.Text.Json;

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

            var newVerse = new Verse { AuthorId = userId, Logo = request.Logo, Name = request.Name, Start = request.Start, End = request.End };

            await _verseRepository.CreateAsync(newVerse);

            var update = Builders<User>.Update.AddToSet(user => user.Verses, newVerse.Id);

            var result = await _users.UpdateOneAsync(x => x.Id == userId, update);

            var response = new VerseCreateResponse
            {
                VerseId = newVerse.Id
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VerseResponse>> Get(string id)
        {
            var userId = HttpContext.GetUserId();

            var verse = await _verseRepository.FindOneAsync(x => x.Id == id);

            if (verse == null)
            {
                return NotFound();
            }

            //if (verse.AuthorId != userId)
            //{
            //    return Forbid();
            //}

            var response = _mapper.Map<VerseResponse>(verse);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<VerseUpdateResponse>> Update(string id, [FromBody] VerseUpdateRequest request)
        {
            var userId = HttpContext.GetUserId();

            var update = Builders<Verse>.Update
                .Set(x => x.Name, request.Name)
                .Set(x => x.Start, request.Start)
                .Set(x => x.End, request.End)
                .Set(x => x.Logo, request.Logo);

            var result = await _verseRepository.UpdateOneAsync(x => x.Id == id && x.AuthorId == userId, update);

            if (result.MatchedCount == 0)
            {
                return NotFound(new { message = "Unable to update the verse. Resource not found." });
            }

            return Ok(new VerseUpdateResponse());
        }

        [HttpPost("{id}/{type}")]
        public async Task<ActionResult<VerseItemUpdateResponse>> AddItem(string id, string type, [FromBody] dynamic item)
        {
            var userId = HttpContext.GetUserId();

            var verse = await _verseRepository.FindOneAsync(x => x.Id == id && x.AuthorId == userId);
            if (verse == null) return Forbid();

            var newId = ObjectId.GenerateNewId().ToString(); ;

            switch (type.ToLower())
            {
                case "categories":
                    Category сategory = _mapper.Map<Category>(JsonSerializer.Deserialize<CategoryDto>(item.ToString()));
                    сategory.Id = newId;
                    verse.Categories.Add(сategory);
                    break;
                case "events":
                    Event newEvent = _mapper.Map<Event>(JsonSerializer.Deserialize<EventDto>(item.ToString()));

                    //if (newEvent.Start < verse.Start)
                    //    return BadRequest("Incorrect Date");

                    newEvent.Id = newId;
                    verse.Events.Add(newEvent);
                    break;
                case "presenters":
                    Presenter presenter = _mapper.Map<Presenter>(JsonSerializer.Deserialize<PresenterDto>(item.ToString()));
                    presenter.Id = newId;
                    verse.Presenters.Add(presenter);
                    break;
                case "locations":
                    Location location = _mapper.Map<Location>(JsonSerializer.Deserialize<LocationDto>(item.ToString()));
                    location.Id = newId;
                    verse.Locations.Add(location);
                    break;
                default:
                    return BadRequest("Invalid item type.");
            }

            await _verseRepository.ReplaceOneAsync(x => x.Id == verse.Id, verse);
            return Ok(new VerseItemUpdateResponse { id = newId });
        }

        [HttpPut("{id}/{type}")]
        public async Task<IActionResult> UpdateItem(string id, string type, [FromBody] dynamic item)
        {
            var userId = HttpContext.GetUserId();

            var verse = await _verseRepository.FindOneAsync(x => x.Id == id && x.AuthorId == userId);
            if (verse == null) return Forbid();
            var index = -1;
            switch (type.ToLower())
            {
                case "categories":
                    Category сategory = _mapper.Map<Category>(JsonSerializer.Deserialize<CategoryDto>(item.ToString()));
                    index = verse.Categories.FindIndex(c => c.Id == сategory.Id);
                    if (index != -1) verse.Categories[index] = сategory;
                    break;
                case "events":
                    Event existingEvent = _mapper.Map<Event>(JsonSerializer.Deserialize<EventDto>(item.ToString()));
                    index = verse.Events.FindIndex(c => c.Id == existingEvent.Id);
                    if (index != -1) verse.Events[index] = existingEvent;
                    break;
                case "presenters":
                    Presenter presenter = _mapper.Map<Presenter>(JsonSerializer.Deserialize<PresenterDto>(item.ToString()));
                    index = verse.Presenters.FindIndex(c => c.Id == presenter.Id);
                    if (index != -1) verse.Presenters[index] = presenter;
                    break;
                case "locations":
                    Location location = _mapper.Map<Location>(JsonSerializer.Deserialize<LocationDto>(item.ToString()));
                    index = verse.Locations.FindIndex(c => c.Id == location.Id);
                    if (index != -1) verse.Locations[index] = location;
                    break;
                default:
                    return BadRequest("Invalid item type.");
            }

            await _verseRepository.ReplaceOneAsync(x => x.Id == verse.Id, verse);
            return Ok();
        }

        [HttpDelete("{id}/{type}/{itemId}")]
        public async Task<IActionResult> DeleteItem(string id, string type, string itemId)
        {
            var userId = HttpContext.GetUserId();

            var verse = await _verseRepository.FindOneAsync(x => x.Id == id && x.AuthorId == userId);
            if (verse == null) return Forbid();

            switch (type.ToLower())
            {
                case "categories":
                    verse.Categories.RemoveAll(c => c.Id == itemId);
                    break;
                case "events":
                    verse.Events.RemoveAll(e => e.Id == itemId);
                    break;
                case "presenters":
                    verse.Presenters.RemoveAll(p => p.Id == itemId);
                    break;
                case "locations":
                    verse.Locations.RemoveAll(l => l.Id == itemId);
                    break;
                default:
                    return BadRequest("Invalid item type.");
            }

            await _verseRepository.ReplaceOneAsync(x => x.Id == verse.Id, verse);
            return Ok();
        }
    }
}
