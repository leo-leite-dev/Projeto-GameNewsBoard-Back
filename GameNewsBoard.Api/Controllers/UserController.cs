using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameNewsBoard.Api.Helpers;
using GameNewsBoard.Application.IServices;
using System.Security.Claims;

namespace GameNewsBoard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return ApiResponseHelper.CreateError("Usuário não autenticado", "Não foi possível identificar o usuário atual.", 401);

                var profile = await _userService.GetUserProfileAsync(userId);
                return Ok(ApiResponseHelper.CreateSuccess(profile, "Sessão válida"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar dados do usuário.");
                return ApiResponseHelper.CreateError("Erro no servidor", "Erro ao validar a sessão do usuário. Tente novamente mais tarde.", 500);
            }
        }
    }
}