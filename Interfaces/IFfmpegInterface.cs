namespace redditJsonTool.Interfaces;

public interface IFfmpegInterface
{
    Task HandleVideoCreationAsync(string videoFilePath, string subtitlesFilePath, string outputVideoPath);
}