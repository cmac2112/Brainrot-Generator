using System.Text;
using System.Text.Json;

namespace redditJsonTool;

public static class TextToSpeech
{
    private static readonly HttpClient client = new HttpClient();
    //text to speech returns an mp3 byte array
    public static async Task<byte[]> ConvertTextToSpeechAsync(RedditPost post, string apiKey)
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
                "https://api.elevenlabs.io/v1/text-to-speech/nPczCjzI2devNBz1zQrb?output_format=mp3_44100_128")
            {
                Content = content
            };
            
            request.Headers.Add(
                "xi-api-key", apiKey);

            request.Headers.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("audio/mpeg")
            );

            var result = await client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                var error = await result.Content.ReadAsStringAsync();
                throw new Exception($"ElevenLabs error ({result.StatusCode}): {error}");
            }

            result.EnsureSuccessStatusCode();
            Console.WriteLine("SUCCESSSUCCESSSUCCESSSUCCESSSUCCESSSUCCESS");
            byte[] audioBytes = await result.Content.ReadAsByteArrayAsync();
            return audioBytes;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"HTTP error: {ex.Message}", ex);
        }

    }
}