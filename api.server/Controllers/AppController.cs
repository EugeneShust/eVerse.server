using API.Server.Models;
using API.Server.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using Shared.Protocol.Requests;
using Internal.Extentions;
using Shared.Protocol.Dtos;

namespace api.server.Controllers
{
    [Route("api/[controller]")]
    public class AppController(IVerseRepository verseRepository, IMapper mapper) : ControllerBase
    {
        private readonly IVerseRepository _verseRepository = verseRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet()]
        public async Task<ActionResult<AppExploreResponse>> Get()
        {
            var verses = await _verseRepository.FindManyAsync(x => x.End >= DateTime.Now);

            if (verses.Count == 0)
            {
                return NotFound();
            }

            var response = new AppExploreResponse
            {
                Apps = verses.Select(x => _mapper.Map<AppPreviewDto>(x)).ToList(),
            };

            return Ok(response);
        }
    }
}