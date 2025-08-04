using Microsoft.Extensions.Logging;

namespace GameNewsBoard.Infrastructure.Helpers
{
    public static class LoggingExecutionHelper
    {
        public static async Task<T> ExecuteWithLoggingAsync<T>(
            Func<Task<T>> action,
            ILogger logger,
            string errorMessage)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, errorMessage);
                throw;
            }
        }
    }
}