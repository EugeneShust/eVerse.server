using AutoMapper;
using API.Server.Models;
using Shared.Protocol.Dtos;

namespace API.Server.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ProfileDto>();
            CreateMap<Verse, VerseDto>();
        }
    }
}