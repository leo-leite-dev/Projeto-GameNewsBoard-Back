using System.Globalization;
using AutoMapper;
using GameNewsBoard.Application.DTOs;
using GameNewsBoard.Application.DTOs.Responses.Game;
using GameNewsBoard.Application.DTOs.Responses.Steam;
using GameNewsBoard.Infrastructure.External.Steam.Dtos;
using GameNewsBoard.Infrastructure.ExternalDtos;
using GameNewsBoard.Infrastructure.ExternalDtos.Steam;
using GameNewsBoard.Infrastructure.Igdb.ExternalDtos;

namespace GameNewsBoard.Application.Mapping
{
    public class ExternalMappingProfile : Profile
    {
        public ExternalMappingProfile()
        {
            // 🔹 Game News
            CreateMap<GameNewsDto, GNewsArticleResponse>()
                .ForMember(dest => dest.PubDate, opt => opt.MapFrom(src =>
                    src.PubDate.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)));

            CreateMap<GNewsResponseWrapper, GNewsResponse>();

            // 🔹 IGDB → RawGameDto
            CreateMap<IgdbGameDto, RawGameDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src =>
                    src.Cover != null && !string.IsNullOrEmpty(src.Cover.Url)
                        ? $"https:{src.Cover.Url}"
                        : string.Empty))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src =>
                    src.AggregatedRating ?? src.UserRating ?? 0))
                .ForMember(dest => dest.ReleaseDateUnix, opt => opt.MapFrom(src => src.FirstReleaseDateUnix))
                .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src =>
                    src.Platforms != null ? src.Platforms.Select(p => p.Name).ToList() : new List<string>()));

            // 🔹 IGDB → GameReleaseResponse PERGUNTAR PARA O GPT SE É NECESSARIO ISSO
            CreateMap<IgdbGameReleaseDto, RawGameReleaseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.CoverImageUrl, opt => opt.MapFrom(src =>
                    src.Cover != null && !string.IsNullOrEmpty(src.Cover.Url)
                        ? $"https:{src.Cover.Url.Replace("t_thumb", "t_cover_big")}"
                        : string.Empty))
                .ForMember(dest => dest.ReleaseDateUnix, opt => opt.MapFrom(src => src.FirstReleaseDate))
                .ForMember(dest => dest.Platforms, opt => opt.MapFrom(src =>
                    src.Platforms != null
                        ? src.Platforms.Select(p => p.Name).ToList()
                        : new List<string>()));

            CreateMap<SteamPlayerDto, SteamUserProfileResponse>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatarfull));

           // 🔹 Steam → SteamUserProfileResponse

            // 🔹 Steam → SteamOwnedGameResponse
            CreateMap<SteamOwnedGameDto, OwnedGameResponse>()
                .ForMember(dest => dest.AppId, opt => opt.MapFrom(src => src.AppId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PlaytimeForever, opt => opt.MapFrom(src => src.PlaytimeForever))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src =>
                    $"https://media.steampowered.com/steamcommunity/public/images/apps/{src.AppId}/{src.IconUrl}.jpg"));

            // 🔹 Steam → SteamAchievementResponse
            CreateMap<SteamAchievementDto, SteamAchievementResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApiName))
                .ForMember(dest => dest.IsUnlocked, opt => opt.MapFrom(src => src.Achieved == 1))
                .ForMember(dest => dest.UnlockedAt, opt => opt.MapFrom(src =>
                    src.UnlockTimeUnix > 0
                        ? DateTimeOffset.FromUnixTimeSeconds(src.UnlockTimeUnix)
                        : (DateTimeOffset?)null));

        }
    }
}
