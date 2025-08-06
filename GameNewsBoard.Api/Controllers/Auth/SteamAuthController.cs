using GameNewsBoard.Application.IRepository;
using GameNewsBoard.Application.IServices.Auth;
using GameNewsBoard.Api.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AspNet.Security.OpenId.Steam;
using GameNewsBoard.Api.Configurations;
using Microsoft.Extensions.Options;
using GameNewsBoard.Application.IServices.ISteam;

namespace GameNewsBoard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SteamAuthController : ControllerBase
    {
        private readonly ISteamAuthService _steamAuthService;
        private readonly ISteamUserService _steamUserService;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        private readonly FrontendSettings _frontendSettings;
        private readonly ILogger<SteamAuthController> _logger;

        public SteamAuthController(
            ISteamAuthService steamAuthService,
            ISteamUserService steamUserService,
            ITokenService tokenService,
            ICookieService cookieService,
            IOptions<FrontendSettings> frontendOptions,
            ILogger<SteamAuthController> logger)
        {
            _steamAuthService = steamAuthService ?? throw new ArgumentNullException(nameof(steamAuthService));
            _steamUserService = steamUserService ?? throw new ArgumentNullException(nameof(steamUserService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _cookieService = cookieService ?? throw new ArgumentNullException(nameof(cookieService));
            _frontendSettings = frontendOptions.Value ?? throw new ArgumentNullException(nameof(frontendOptions.Value));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("steam-login")]
        public IActionResult LoginWithSteam()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "/api/SteamAuth/callback"
            };

            return Challenge(props, SteamAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("/signin-steam")]
        public async Task<IActionResult> SigninSteam()
        {
            return Redirect("/api/SteamAuth/callback");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                if (!result.Succeeded || result.Principal == null)
                {
                    _logger.LogWarning("Autenticação com Steam falhou.");
                    return ApiResponseHelper.CreateError("Autenticação falhou", "Não foi possível autenticar com a Steam.", 401);
                }

                var steamUrl = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var steamId = steamUrl?.Split('/').LastOrDefault();

                if (string.IsNullOrEmpty(steamId))
                {
                    _logger.LogWarning("SteamId não encontrado na resposta da Steam.");
                    return ApiResponseHelper.CreateError("SteamId ausente", "Não foi possível obter o SteamId da resposta.", 400);
                }

                var jwtUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(jwtUserIdClaim, out var authenticatedUserId))
                {
                    _logger.LogInformation("Usuário autenticado está vinculando conta Steam: {SteamId}", steamId);
                    var resultLink = await _steamUserService.LinkSteamAccountAsync(authenticatedUserId, steamId);

                    if (!resultLink.IsSuccess)
                    {
                        _logger.LogWarning("Falha ao vincular SteamId ao usuário logado: {Reason}", resultLink.Error);
                        return ApiResponseHelper.CreateError("Falha ao vincular conta Steam", resultLink.Error, 400);
                    }

                    return Redirect(_frontendSettings.RedirectAfterSteamLogin);
                }

                var user = await _steamAuthService.AuthenticateOrCreateSteamUserAsync(steamId);

                var token = _tokenService.GenerateToken(user);
                _cookieService.SetJwtCookie(Response, token, TimeSpan.FromHours(1));

                _logger.LogInformation("Usuário autenticado via Steam com sucesso. Username: {Username}", user.Username);
                return Redirect(_frontendSettings.RedirectAfterSteamLogin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no callback de autenticação Steam.");
                return ApiResponseHelper.CreateError("Erro no servidor", "Ocorreu um erro ao processar a autenticação com a Steam.", 500);
            }
        }
    }
}