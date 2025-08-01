using GameNewsBoard.Api.Helpers;
using GameNewsBoard.Application.IServices;
using GameNewsBoard.Domain.Enums;
using GameNewsBoard.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameNewsBoard.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadedImageController : ControllerBase
    {
        private readonly IUploadedImageService _uploadedImageService;
        private readonly ILogger<UploadedImageController> _logger;

        public UploadedImageController(
            IUploadedImageService uploadedImageService,
            ILogger<UploadedImageController> logger)
        {
            _uploadedImageService = uploadedImageService ?? throw new ArgumentNullException(nameof(uploadedImageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(ApiResponseHelper.CreateError("Imagem ausente", "Nenhuma imagem foi fornecida para upload."));

            if (image.Length > 5 * 1024 * 1024)
                return BadRequest(ApiResponseHelper.CreateError("Imagem muito grande", "A imagem excede o tamanho máximo permitido de 5MB."));

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(image.ContentType))
                return BadRequest(ApiResponseHelper.CreateError("Tipo inválido", "O tipo de imagem fornecido não é suportado. Use JPG, PNG ou WebP."));

            try
            {
                var userId = User.GetUserId();
                await using var stream = image.OpenReadStream();

                var result = await _uploadedImageService.RegisterImageAsync(userId, stream, image.FileName, image.ContentType, ImageBucketCategory.Tier);

                if (!result.IsSuccess || result.Value is null)
                    return ApiResponseHelper.CreateError("Falha no upload", result.Error ?? "Erro ao registrar a imagem.");

                return Ok(ApiResponseHelper.CreateSuccess(new
                {
                    imageId = result.Value.Id,
                    imageUrl = result.Value.Url
                }, "Imagem enviada com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar imagem");
                return StatusCode(500, ApiResponseHelper.CreateError("Erro interno", "Ocorreu um erro inesperado ao enviar a imagem."));
            }
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var userId = User.GetUserId();

            var result = await _uploadedImageService.DeleteImageAsync(userId, id);

            if (!result.IsSuccess)
                return ApiResponseHelper.CreateError("Erro ao deletar imagem", result.Error);

            return Ok(ApiResponseHelper.CreateSuccess("Imagem deletada com sucesso"));
        }
    }
}