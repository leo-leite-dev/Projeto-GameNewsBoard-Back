using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices;
using GameNewsBoard.Application.IServices.Auth;
using GameNewsBoard.Application.IServices.IGame;
using GameNewsBoard.Application.IServices.Igdb;
using GameNewsBoard.Application.IServices.ISteam;
using GameNewsBoard.Application.Services;
using GameNewsBoard.Application.Services.Auth;
using GameNewsBoard.Domain.IStorage;
using GameNewsBoard.Infrastructure.External.Igdb;
using GameNewsBoard.Infrastructure.Repositories;
using GameNewsBoard.Infrastructure.Services;
using GameNewsBoard.Infrastructure.Services.Auth;
using GameNewsBoard.Infrastructure.Services.Igdb;
using GameNewsBoard.Infrastructure.Services.News;
using GameNewsBoard.Infrastructure.Services.Steam;
using Microsoft.Extensions.DependencyInjection;

namespace GameNewsBoard.Infrastructure.Configurations
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IGNewsService, GNewsService>();
            services.AddScoped<IIgdbGameReleaseService, IgdbGameReleaseService>();

            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IGameRepository, GameRepository>();

            //Auth
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();

            //TierList
            services.AddScoped<ITierListService, TierListService>();
            services.AddScoped<ITierListRepository, TierListRepository>();

            //Status
            services.AddScoped<IGameStatusService, GameStatusService>();
            services.AddScoped<IGameStatusRepository, GameStatusRepository>();

            services.AddScoped<IUploadedImageService, UploadedImageService>();
            services.AddScoped<IUploadedImageRepository, UploadedImageRepository>();
            services.AddScoped<IImageStorageService, SupabaseImageStorageService>();


            services.AddScoped<IIgdbApiService, IgdbApiService>();
            services.AddScoped<IIgdbQueryBuilder, IgdbQueryBuilder>();

            services.AddScoped<ISteamApiService, SteamApiService>();
            services.AddScoped<ISteamUserService, SteamUserService>();
            services.AddScoped<ISteamAuthService, SteamAuthService>();
        }
    }
}