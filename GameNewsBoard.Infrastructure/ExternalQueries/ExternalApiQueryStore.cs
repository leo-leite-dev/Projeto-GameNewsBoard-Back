using System.Text;
using GameNewsBoard.Domain.Enums;

namespace GameNewsBoard.Infrastructure.Queries
{
    public static class ExternalApiQueryStore
    {
        public static class Igdb
        {
            public static string GenerateGamesQuery(int page, int pageSize)
            {
                return $@"
                    fields name, aggregated_rating, rating, first_release_date, cover.url, platforms.name;
                    where rating != null & platforms != null & cover != null;
                    sort aggregated_rating desc;
                    limit {pageSize};
                    offset {(page - 1) * pageSize};";
            }

            public static string GenerateGamesQueryWithOffset(int offset, int pageSize)
            {
                return $@"
                    fields name, aggregated_rating, rating, first_release_date, cover.url, platforms.name;
                    where rating != null & platforms != null & cover != null;
                    sort aggregated_rating desc;
                    limit {pageSize};
                    offset {offset};";
            }

            public static string GenerateGamesReleasedBetweenQuery(long startUnix, long endUnix, int limit = 500)
            {
                var whereClause = $"first_release_date >= {startUnix} & first_release_date <= {endUnix}";

                var sb = new StringBuilder();
                sb.AppendLine("fields name, cover.url, platforms.id, platforms.name, first_release_date, category;");
                sb.AppendLine($"where {whereClause};");
                sb.AppendLine("sort first_release_date asc;");
                sb.AppendLine($"limit {limit};");

                return sb.ToString();
            }
        }
    }
}