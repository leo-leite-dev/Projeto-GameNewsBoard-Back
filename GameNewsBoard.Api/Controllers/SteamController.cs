using GameNewsBoard.Api.Helpers;
using GameNewsBoard.Application.IServices.ISteam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameNewsBoard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SteamController : ControllerBase
    {
        private readonly ISteamUserService _steamUserService;
        private readonly ILogger<SteamController> _logger;

        public SteamController(ISteamUserService steamUserService, ILogger<SteamController> logger)
        {
            _steamUserService = steamUserService ?? throw new ArgumentNullException(nameof(steamUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("steam/profile")]
        [Authorize]
        public async Task<IActionResult> GetSteamProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return ApiResponseHelper.CreateError("Usuário não autenticado", "Não foi possível identificar o usuário atual.", 401);

                var profile = await _steamUserService.GetCompleteSteamUserProfileAsync(userId);

                return Ok(ApiResponseHelper.CreateSuccess(profile, "Perfil da Steam obtido com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter perfil da Steam para usuário logado.");

                return ApiResponseHelper.CreateError(
                    "Erro ao buscar dados da Steam",
                    "Não foi possível obter os dados da Steam. Tente novamente mais tarde.",
                    500
                );
            }
        }

        [HttpPost("link")]
        [Authorize]
        public async Task<IActionResult> LinkSteamAccount([FromBody] string steamId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return ApiResponseHelper.CreateError("Usuário não autenticado", "Não foi possível identificar o usuário atual.", 401);

                var result = await _steamUserService.LinkSteamAccountAsync(userId, steamId);
                if (!result.IsSuccess)
                    return ApiResponseHelper.CreateError("Erro ao vincular conta", result.Error);

                return Ok(ApiResponseHelper.CreateSuccess("Conta Steam vinculada com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao vincular conta Steam.");
                return ApiResponseHelper.CreateError("Erro ao vincular conta", "Ocorreu um erro ao tentar vincular a conta Steam.", 500);
            }
        }
    }
}
