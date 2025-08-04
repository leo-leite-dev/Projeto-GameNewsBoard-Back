using System.Globalization;
using AutoMapper;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.DTOs.Responses;
using GameNewsBoard.Application.Responses.DTOs.Responses;
using GameNewsBoard.Domain.Entities;

namespace GameNewsBoard.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RawGameDto, Game>()
                .ForMember(dest => dest.Platform, opt => opt.MapFrom(src =>
                    src.Platforms != null && src.Platforms.Any()
                        ? string.Join(", ", src.Platforms)
                        : "Unknown"))
                .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating ?? 0))
                .ForMember(dest => dest.Released, opt => opt.MapFrom(src =>
                    src.ReleaseDateUnix.HasValue
                        ? DateTimeOffset.FromUnixTimeSeconds(src.ReleaseDateUnix.Value)
                        : DateTimeOffset.MinValue))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<RawGameDto, GameResponse>()
                .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.Platform, opt => opt.MapFrom(src =>
                    src.Platforms != null && src.Platforms.Any()
                        ? string.Join(", ", src.Platforms)
                        : "Unknown"))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating ?? 0))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src =>
                    src.ReleaseDateUnix.HasValue
                        ? DateTimeOffset.FromUnixTimeSeconds(src.ReleaseDateUnix.Value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                        : string.Empty));

            CreateMap<RawGameDto, GameReleaseResponse>()
                .ForMember(dest => dest.Platform, opt => opt.MapFrom(src =>
                    src.Platforms != null && src.Platforms.Any()
                        ? string.Join(", ", src.Platforms)
                        : "Unknown"))
                .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src =>
                    src.ReleaseDateUnix.HasValue
                        ? DateTimeOffset.FromUnixTimeSeconds(src.ReleaseDateUnix.Value).ToString("yyyy-MM-dd")
                        : string.Empty))
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<GameResponse, Game>()
                .ForMember(dest => dest.Released, opt => opt.MapFrom(src => ParseDate(src.ReleaseDate)))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Game, GameDTO>()
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Released.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));

            CreateMap<Game, GameResponse>()
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.Released.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));

            CreateMap<TierList, TierListResponse>();
            CreateMap<TierListEntry, TierListEntryResponse>();

            CreateMap<User, UserProfileResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<GameStatus, GameStatusResponse>()
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId))
                .ForMember(dest => dest.Game, opt => opt.MapFrom(src => src.Game))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }

        private static DateTimeOffset ParseDate(string released)
        {
            if (DateTimeOffset.TryParseExact(released, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            return DateTimeOffset.MinValue;
        }
    }
}