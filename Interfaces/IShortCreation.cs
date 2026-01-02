using redditJsonTool.Types;

namespace redditJsonTool.Interfaces;

public interface IShortCreation
{
    Task<Either<ErrorMessage, Success>> HandleVideoCreationAsync(string apikey, string openaiKey, RedditPost redditPost,
        string videoFilePath, string subtitlesFilePath, string outputVideoPath);
    
    Task<Either<ErrorMessage, RedditPost>> GetAndParseRedditPostAsync(string url);
}