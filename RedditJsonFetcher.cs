namespace redditJsonTool;

public static class RedditJsonFetcher
{
    public static async Task<string> FetchRedditJsonAsync(string url)
    {
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "RedditJsonParser/v0.0.1 (by /u/cmac2112)");

            var json = await client.GetStringAsync(url);
            return json;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}