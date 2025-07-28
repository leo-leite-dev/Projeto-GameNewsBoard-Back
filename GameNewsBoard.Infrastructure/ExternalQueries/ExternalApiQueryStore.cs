using System.Text;

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

            public static string GenerateGamesReleasedBetweenQuery(
                long startUnix, long endUnix, IEnumerable<int>? platformIds = null,
                int limit = 500)
            {
                var whereClause = new StringBuilder();
                whereClause.Append($"first_release_date >= {startUnix} & first_release_date <= {endUnix}");

                if (platformIds != null && platformIds.Any())
                {
                    var platformList = string.Join(",", platformIds);
                    whereClause.Append($" & platforms != null & platforms = ({platformList})");
                }

                var sb = new StringBuilder();
                sb.AppendLine("fields name, cover.url, platforms.id, platforms.name, first_release_date, category;");
                sb.AppendLine($"where {whereClause};");
                sb.AppendLine("sort first_release_date desc;");
                sb.AppendLine($"limit {limit};");

                return sb.ToString();
            }
        }
    }
}