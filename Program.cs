using redditJsonTool.Interfaces;
using redditJsonTool.Types;

namespace redditJsonTool;

class Program
{
    public IRedditInterface? RedditInterface { get; set; }
    public IFfmpegInterface? FfmpegInterface { get; set; }

    async Task Main(string[] args)
    {

        
        
        
        // change this to read from args and ouput directory later
        var url =
            "https://www.reddit.com/r/AskReddit/comments/1pi3cju/professionals_who_enter_peoples_homes_plumbers/.json";
        Console.WriteLine(url);

        var apikey = Environment.GetEnvironmentVariable("XI_API_KEY");
        if (string.IsNullOrEmpty(apikey))
        {
            Console.WriteLine("XI_API_KEY environment variable is not set.");
            return;
        }

        //var jsonResult = await RedditJsonFetcher.FetchRedditJsonAsync(url);
        
        //now we have the json, we need to parse out the initial title, and all of the subsequent replies of the initial post
        // none of the replies to the replies are needed.

        //var parsedResult = RedditJsonFetcher.ParseRedditJson(jsonResult);

        if (RedditInterface != null)
        {
            var parsedResult = await RedditInterface.GetAndParseRedditPostAsync(args[0]);

            switch (parsedResult)
            {
                case Left<ErrorMessage, RedditPost> left:
                    Console.WriteLine($"Error fetching Reddit post: {left.Value.Message}");
                    return;
            }
           
            Console.WriteLine($"Title: {parsedResult.Title}");
        }
        else
        {
            throw new Exception("RedditInterface is not set.");
        }

        //use the xi api to create text to speech mp3 of the content
        //then eventually add the ability where this generates a video dubbed over with the tts audio
        //var result = await TextToSpeech.ConvertTextToSpeechAsync(parsedResult, apikey);
        
        //to prevent spamming the api during testing, we will read from a local mp3 file instead
        var result = await File.ReadAllBytesAsync("sample_input.mp3");
        
        //download the mp3 file
        //await File.WriteAllBytesAsync("output.mp3", result);
        
        // the ai voice ouput is slow, use ffmpeg to speed it up by 1.3x
        await FfmpegHelper.SpeedUpMp3FileAsync(result, "output.mp3", 1.3);
        
        await FfmpegHelper.AddAudioToVideoAsync("output.mp3", "videoplayback.mp4", "final_output.mp4");
        
        await FfmpegHelper.BurnSubtitlesIntoVideoAsync("final_output.mp4", "subtitles.srt", "final_video_with_subtitles.mp4");
        //convert the mp3 to an srt file
        
        
        
        Console.WriteLine("MP3 file saved as output.mp3 and sped up by 1.5x.");
    }
}