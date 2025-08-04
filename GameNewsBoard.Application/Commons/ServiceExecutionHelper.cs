using GameNewsBoard.Domain.Commons;
using Microsoft.EntityFrameworkCore;
namespace GameNewsBoard.Application.Commons
{
    public static class ServiceExecutionHelper
    {
        public static async Task<Result> TryExecuteAsync(Func<Task> action, string fallbackErrorMessage)
        {
            try
            {
                await action();
                return Result.Success();
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Result.Failure("Erro ao salvar dados no banco de dados.");
            }
            catch (Exception)
            {
                return Result.Failure(fallbackErrorMessage);
            }
        }

        public static async Task<Result<T>> TryExecuteAsync<T>(Func<Task<T>> action, string fallbackErrorMessage)
        {
            try
            {
                var result = await action();
                return Result<T>.Success(result);
            }
            catch (ArgumentException ex)
            {
                return Result<T>.Failure(ex.Message);
            }
            catch (DbUpdateException)
            {
                return Result<T>.Failure("Erro ao salvar dados no banco de dados.");
            }
            catch (Exception)
            {
                return Result<T>.Failure(fallbackErrorMessage);
            }
        }
    }
}