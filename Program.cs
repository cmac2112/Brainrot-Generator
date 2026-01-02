using System.Security.Authentication.ExtendedProtection;
using redditJsonTool.Interfaces;
using redditJsonTool.Types;
using Microsoft.Extensions.DependencyInjection;
using redditJsonTool.Interfaces.Implementations;

namespace redditJsonTool;

class Program
{
    //background video to use, find the most brainrot video possible like a subway surfers gameplay or something
    private const string _videoPath = "input.mp4"; 
    //path to save subtitles temporarily
    private const string _subtitlesPath = "subtitles.srt";
    //path to save output video, could use a rework on the file name scheme
    private static string _outputVideoPath => $"Output/Output_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
    static async Task Main(string[] args)
    {

        // inject the short creation service.
        // why an interface for such a simple app? future proofing for later expansion.
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IShortCreation, ShortCreationImplementation>()
            .BuildServiceProvider();

        var shortCreation = serviceProvider.GetRequiredService<IShortCreation>();
        
        //get the required api keys, see README for launchSettings.json setup
        var apikey = Environment.GetEnvironmentVariable("XI_API_KEY");
        if (string.IsNullOrEmpty(apikey))
        {
            Console.WriteLine("XI_API_KEY environment variable is not set.");
            return;
        }

        var openaiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(openaiKey))
        {
            Console.WriteLine("OPENAI_API_KEY environment variable is not set.");
            return;
        }

    
        var parsedResult = await shortCreation.GetAndParseRedditPostAsync(args[0]);

        var res = parsedResult.TryGetRight(out RedditPost? redditPost);
        if (res && redditPost != null && !redditPost.Title.IsWhiteSpace())
        {
            //we have a valid post
            Console.WriteLine("Title: " + redditPost.Title);
            
            var outputPath = await shortCreation.HandleVideoCreationAsync(apikey, openaiKey, redditPost, _videoPath,
                _subtitlesPath, _outputVideoPath);
            if(outputPath.TryGetRight(out _))
            {
                Console.WriteLine("Video created successfully at: " + _outputVideoPath);
            }
            else if(outputPath.TryGetLeft(out var error))
            {
                Console.WriteLine("Error during video creation: " + error?.Message);
            }

        }
        else
        {
            throw new Exception("Failed to fetch or parse Reddit post");
        }

    }
    }
