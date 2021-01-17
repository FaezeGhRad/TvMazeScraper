using AutoMapper;
using TvMaze.Scraper.Abstractions.Entities;
using TvMaze.Scraper.API.Models;

namespace TvMaze.Scraper.API.Mappers
{
    public class ShowMapperProfile : Profile
    {
        public ShowMapperProfile()
        {
            CreateMap<ShowEntity, ShowDto>();

            CreateMap<CastEntity, CastDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Person.Id))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Person.Name))
                .ForMember(d => d.Birthday, o => o.MapFrom(s => s.Person.Birthday.HasValue
                ? s.Person.Birthday.Value.ToString("yyyy-M-dd")
                : string.Empty));
        }
    }
}
