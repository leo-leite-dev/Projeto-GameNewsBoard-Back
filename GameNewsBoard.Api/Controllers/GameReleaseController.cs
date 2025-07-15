using Microsoft.AspNetCore.Mvc;
using GameNewsBoard.Application.IServices;
using GameNewsBoard.Api.Helpers;
using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameReleaseController : ControllerBase
{
    private readonly IGameReleaseService _gameReleaseService;
    private readonly ILogger<GameReleaseController> _logger;

    public GameReleaseController(IGameReleaseService gameReleaseService, ILogger<GameReleaseController> logger)
    {
        _gameReleaseService = gameReleaseService;
        _logger = logger;
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayReleases(
    [FromQuery] PlatformFamily? platform = null)
    {
        try
        {
            var releases = await _gameReleaseService.GetTodayReleasesGamesAsync(platform);

            if (releases == null || releases.Count == 0)
                return Ok(ApiResponseHelper.CreateEmptySuccess("Nenhum jogo será lançado hoje."));

            return Ok(ApiResponseHelper.CreateSuccess(releases, "Jogos lançados hoje carregados com sucesso."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar lançamentos do dia.");
            return ApiResponseHelper.CreateError("Erro ao buscar lançamentos do dia", ex.Message);
        }
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingReleases(
        [FromQuery] int daysAhead = 7,
        [FromQuery] PlatformFamily? platform = null)
    {
        try
        {
            var releases = await _gameReleaseService.GetUpcomingGamesAsync(daysAhead, platform);

            if (releases == null || releases.Count == 0)
                return Ok(ApiResponseHelper.CreateEmptySuccess($"Nenhum jogo será lançado nos próximos {daysAhead} dias."));

            return Ok(ApiResponseHelper.CreateSuccess(releases, $"Jogos que serão lançados nos próximos {daysAhead} dias carregados com sucesso."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar lançamentos futuros.");
            return ApiResponseHelper.CreateError("Erro ao buscar lançamentos futuros", ex.Message);
        }
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentReleases(
      [FromQuery] int daysBack = 7,
      [FromQuery] PlatformFamily? platform = null)
    {
        try
        {
            var releases = await _gameReleaseService.GetRecentlyReleasedGamesAsync(daysBack, platform);

            if (releases == null || releases.Count == 0)
                return Ok(ApiResponseHelper.CreateEmptySuccess($"Nenhum jogo foi lançado nos últimos {daysBack} dias."));

            return Ok(ApiResponseHelper.CreateSuccess(releases, $"Jogos lançados nos últimos {daysBack} dias carregados com sucesso."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar lançamentos recentes.");
            return ApiResponseHelper.CreateError("Erro ao buscar lançamentos recentes", ex.Message);
        }
    }
}
