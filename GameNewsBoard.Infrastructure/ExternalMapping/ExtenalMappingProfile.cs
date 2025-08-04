using System.Globalization;
using AutoMapper;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.Responses.DTOs.Responses;
using GameNewsBoard.Infrastructure.ExternalDtos;
using GameNewsBoard.Infrastructure.Igdb.ExternalDtos;

namespace GameNewsBoard.Application.Mapping
{
    public class ExternalMappingProfile : Profile
    {
        public ExternalMappingProfile()
        {
            // Game News
            CreateMap<GameNewsDto, GameNewsArticleResponse>()
                        .ForMember(dest => dest.PubDate, opt => opt.MapFrom(src => src.PubDate.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)));

            CreateMap<GNewsResponseWrapper, GameNewsResponse>();

            CreateMap<IgdbGameDto, RawGameDto>()
              .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src =>
                  src.Cover != null && !string.IsNullOrEmpty(src.Cover.Url)
                      ? $"https:{src.Cover.Url}"
                      : string.Empty))
              .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.AggregatedRating ?? src.UserRating ?? 0))
              .ForMember(dest => dest.ReleaseDateUnix, opt => opt.MapFrom(src => src.FirstReleaseDateUnix))
              .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src =>
                  src.Platforms != null ? src.Platforms.Select(p => p.Name).ToList() : new List<string>()));

            CreateMap<IgdbGameReleaseDto, GameReleaseResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Platform, opt => opt.MapFrom(src =>
                    src.Platforms != null && src.Platforms.Any()
                        ? string.Join(", ", src.Platforms.Select(p => p.Name))
                        : "Unknown"))
                .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src =>
                    src.Cover != null && !string.IsNullOrEmpty(src.Cover.Url)
                        ? $"https:{src.Cover.Url.Replace("t_thumb", "t_cover_big")}"
                        : string.Empty))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src =>
                    src.FirstReleaseDate.HasValue
                        ? DateTimeOffset
                            .FromUnixTimeSeconds(src.FirstReleaseDate.Value)
                            .UtcDateTime
                            .ToString("yyyy-MM-dd")
                        : "Unknown"))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
        }
    }
}