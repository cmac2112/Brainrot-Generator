using redditJsonTool.Types;

namespace redditJsonTool.Interfaces;

public interface IRedditInterface
{
    Task<Either<ErrorMessage, RedditPost>> GetAndParseRedditPostAsync(string url);
}