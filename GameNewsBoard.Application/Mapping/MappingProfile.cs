using System.Globalization;
using AutoMapper;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.DTOs.Responses;
using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Application.DTOs.Responses.TierList;
using GameNewsBoard.Application.DTOs.Responses.User;
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

            CreateMap<RawGameReleaseDto, GameReleaseResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src => src.CoverImageUrl))
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src =>
                    src.ReleaseDateUnix.HasValue
                        ? DateTimeOffset.FromUnixTimeSeconds(src.ReleaseDateUnix.Value).UtcDateTime.ToString("yyyy-MM-dd")
                        : "Unknown"))
                .ForMember(dest => dest.Platform, opt => opt.MapFrom(src =>
                    src.Platforms != null && src.Platforms.Any()
                        ? string.Join(", ", src.Platforms)
                        : "Unknown"));

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
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));

            CreateMap<GameStatus, GameStatusResponse>();

            CreateMap<UserProfileDto, UserProfileResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.Tiers, opt => opt.MapFrom(src => src.Tiers))
                .ForMember(dest => dest.GameStatuses, opt => opt.MapFrom(src => src.GameStatuses));

            CreateMap<UploadedImage, UploadedImageResponse>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.Id.ToString()));
        }

        private static DateTimeOffset ParseDate(string released)
        {
            if (DateTimeOffset.TryParseExact(released, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            return DateTimeOffset.MinValue;
        }
    }
}