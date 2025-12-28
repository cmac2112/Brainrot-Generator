namespace redditJsonTool;

static class Program
{
    static async Task Main(string[] args)
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

        var jsonResult = await RedditJsonFetcher.FetchRedditJsonAsync(url);
        
        //now we have the json, we need to parse out the initial title, and all of the subsequent replies of the initial post
        // none of the replies to the replies are needed.

        var parsedResult = RedditJsonParser.ParseRedditJson(jsonResult);
        Console.WriteLine($"Title: {parsedResult.Title}");
        
        //use the xi api to create text to speech mp3 of the content
        //then eventually add the ability where this generates a video dubbed over with the tts audio
        var result = await TextToSpeech.ConvertTextToSpeechAsync(parsedResult, apikey);
        
        //download the mp3 file
        await File.WriteAllBytesAsync("output.mp3", result);
        Console.WriteLine("MP3 file saved as output.mp3");
    }
}