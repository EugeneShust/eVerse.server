using AutoMapper;
using API.Server.Models;
using Shared.Protocol.Dtos;
using Shared.Protocol.Requests;
using MongoDB.Bson;

namespace API.Server.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, ProfileResponse>().ForMember(
                dest => dest.Favorites,
                opt => opt.MapFrom(src => src.Favorites
                    .Where(e => e.IsActive)
                    .Select(e => e)
                    .ToList())
            );

            CreateMap<Favorite, FavoriteDto>();

            CreateMap<Verse, VersePreviewDto>();
            CreateMap<Verse, VerseResponse>();
            CreateMap<Verse, AppPreviewDto>();

            CreateMap<Location, LocationDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Presenter, PresenterDto>();

            CreateMap<Event, EventDto>();

            CreateMap<LocationCoords, LocationCoordsDto>();
            CreateMap<LocationCoordsDto, LocationCoords>();

            CreateMap<LocationDto, Location>();
            CreateMap<CategoryDto, Category>();
            CreateMap<PresenterDto, Presenter>();
            CreateMap<EventDto, Event>();
        }
    }
}