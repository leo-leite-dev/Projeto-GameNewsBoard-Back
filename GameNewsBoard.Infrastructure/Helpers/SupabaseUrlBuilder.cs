namespace GameNewsBoard.Infrastructure.Helpers
{
    public static class SupabaseUrlBuilder
    {
        public static string BuildUploadUrl(string baseUrl, string bucket, string fileName)
        {
            var builder = new UriBuilder(baseUrl)
            {
                Path = $"storage/v1/object/{bucket}/{fileName}",
                Query = "upsert=true"
            };

            return builder.Uri.ToString();
        }

        public static string BuildDeleteUrl(string baseUrl, string bucket, string fileName)
        {
            var builder = new UriBuilder(baseUrl)
            {
                Path = $"storage/v1/object/{bucket}/{fileName}"
            };

            return builder.Uri.ToString();
        }

        public static string BuildPublicUrl(string baseUrl, string bucket, string fileName)
        {
            var builder = new UriBuilder(baseUrl)
            {
                Path = $"storage/v1/object/public/{bucket}/{fileName}"
            };

            return builder.Uri.ToString();
        }
    }
}