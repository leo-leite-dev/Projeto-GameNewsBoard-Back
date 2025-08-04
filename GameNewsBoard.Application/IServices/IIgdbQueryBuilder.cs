
namespace GameNewsBoard.Application.IServices
{
    public interface IIgdbQueryBuilder
    {
        string GenerateGamesQuery(int page, int pageSize);
        string GenerateGamesQueryWithOffset(int offset, int pageSize);
        string GenerateGamesReleasedBetweenQuery(long startUnix, long endUnix, IEnumerable<int>? platformIds = null, int limit = 500);
    }
}