using System.Text;
using System.Text.Json;
using redditJsonTool.Types;

namespace redditJsonTool.Interfaces.Implementations;

public class ShortCreationImplementation : IShortCreation
{
    
    private readonly HttpClient _client = new HttpClient();
    private readonly string _elevenLabsApiUrl = "https://api.elevenlabs.io/v1/text-to-speech/nPczCjzI2devNBz1zQrb?output_format=mp3_44100_128";
    private readonly string _openAiWhisperApiUrl = "https://api.openai.com/v1/audio/transcriptions";
    private readonly string _tempAudioPath = "output.mp3";
    private readonly string _tempSrtPath = "subtitles.srt";
    private readonly string _tempVideoPath = "unprocessed.mp4";
    
    private readonly string _archiveAudioPath = "Archive/audio";
    private readonly string _archiveSrtPath = "Archive/srt";
    
    private readonly double _speedFactor = 1.3;
    public async Task<Either<ErrorMessage, Success>> HandleVideoCreationAsync(string apikey, string openaiKey, RedditPost redditPost, string inputvideoFilePath, string subtitlesFilePath, string finaloutputVideoPath)
    {
        //1 get tts from elevenlabs
        await ConvertTextToSpeechAsync(redditPost, apikey);
        
        
        //2 get srt files from whisper openai
        await ConvertSpeechToTextAsync(openaiKey, _tempAudioPath);
        //3 burn into video with ffmpeg
        await FfmpegHelper.AddAudioToVideoAsync(inputvideoFilePath, _tempAudioPath, _tempVideoPath);
        await FfmpegHelper.BurnSubtitlesIntoVideoAsync(_tempVideoPath, _tempSrtPath, finaloutputVideoPath);
        //4 output video
        //5 profit

        return new Success();
    }
    
    private async Task<string> ConvertSpeechToTextAsync(string openAiApiKey,  string audioFilePath)
    {
        //use openai whisper api to convert speech to text
        try
        {
            using var multipartContent = new MultipartFormDataContent();    
            
            var fileStream = File.OpenRead(audioFilePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/mpeg");
            multipartContent.Add(fileContent, "file", Path.GetFileName(audioFilePath));

            multipartContent.Add(new StringContent("whisper-1"), "model");
            //look into trying out vtt here later
            multipartContent.Add(new StringContent("srt"), "response_format");
            using var request = new HttpRequestMessage(HttpMethod.Post, _openAiWhisperApiUrl)
            {
                Content = multipartContent
            };
            request.Headers.Add("Authorization","Bearer " + openAiApiKey);
            
            var result = await _client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                var error = await result.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI Whisper error ({result.StatusCode}): {error}");
            }
            result.EnsureSuccessStatusCode();
            var srtContent = await result.Content.ReadAsStringAsync();
            Directory.CreateDirectory(_archiveSrtPath);
            
            var srtTask = File.WriteAllTextAsync(_tempSrtPath, srtContent);
            var archivetask = File.WriteAllTextAsync(_archiveSrtPath + $"/srt_{DateTime.Now:yyyyMMdd_HHmmss}.srt", srtContent);
            await Task.WhenAll(srtTask, archivetask);
            return _tempSrtPath;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to convert speech to text: " + ex.Message);
        }
    }

    /// <summary>
    /// gets the json from reddit and parses it into a RedditPost object
    /// does not account for bot messages that appear at the top currently so beware, results may vary until i expand upon this
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<Either<ErrorMessage, RedditPost>> GetAndParseRedditPostAsync(string url)
    {
        var redditPost = new RedditPost();
        try
        {
            _client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "RedditJsonParser/v0.0.1 (by /u/cmac2112)");

            var json = await _client.GetStringAsync(url);
            
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
            return new ErrorMessage("Failed to fetch or parse Reddit post: " + ex.Message);
        }
    }
    
    
    /// <summary>
    /// tts with elevenlabs
    /// </summary>
    /// <param name="post">post to translate</param>
    /// <param name="apiKey">api key for elevenlabs</param>
    /// <returns>relative output path</returns>
    /// <exception cref="Exception">http error</exception>
    public async Task<string> ConvertTextToSpeechAsync(RedditPost post, string apiKey)
    {
        try
        {
            var body = new
            {
                text = $"{post.Title}, {post.Content}, {string.Join(", ", post.Comments)}",
                model_id = "eleven_multilingual_v2",
            };

            var json = JsonSerializer.Serialize(body);
            using var content = new StringContent(
                json,
                encoding:Encoding.UTF8,
                "application/json");

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                _elevenLabsApiUrl)
            {
                Content = content
            };
            
            request.Headers.Add(
                "xi-api-key", apiKey);

            request.Headers.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("audio/mpeg")
            );

            var result = await _client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                var error = await result.Content.ReadAsStringAsync();
                throw new Exception($"ElevenLabs error ({result.StatusCode}): {error}");
            }

            result.EnsureSuccessStatusCode();
            byte[] audioBytes = await result.Content.ReadAsByteArrayAsync();

            //save the file to disk
            var archiveTask = File.WriteAllBytesAsync(_archiveAudioPath + $"/audio_{DateTime.Now:yyyyMMdd_HHmmss}.mp3", audioBytes);
            var saveTask = File.WriteAllBytesAsync(_tempAudioPath, audioBytes);
            await Task.WhenAll(archiveTask, saveTask);
            await FfmpegHelper.SpeedUpMp3FileAsync(audioBytes, _tempAudioPath, _speedFactor);
            return _tempAudioPath;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"HTTP error: {ex.Message}", ex);
        }

    }
}