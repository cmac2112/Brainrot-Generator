namespace redditJsonTool;

public static class RedditJsonParser
{
    public static RedditPost ParseRedditJson(string json)
    {
        var redditPost = new RedditPost();
        try
        {
            var jsonArray = System.Text.Json.JsonDocument.Parse(json).RootElement;
            var postData = jsonArray[0].GetProperty("data").GetProperty("children")[0].GetProperty("data");

            redditPost.Title = postData.GetProperty("title").GetString() ?? "";
            redditPost.Content = postData.GetProperty("selftext").GetString() ?? "";

            var commentsArray = jsonArray[1].GetProperty("data").GetProperty("children");
            foreach (var comment in commentsArray.EnumerateArray().Take(3))
            {
                if (comment.GetProperty("kind").GetString() == "t1")
                {
                    var commentData = comment.GetProperty("data");
                    var commentBody = commentData.GetProperty("body").GetString() ?? "";
                    redditPost.Comments.Add(commentBody);
                }
            }

            return redditPost;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}