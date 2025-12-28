namespace redditJsonTool;

public class RedditPost
{
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public List<string> Comments { get; set; } = new List<string>();
}
