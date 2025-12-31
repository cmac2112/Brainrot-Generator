using System.Diagnostics;

namespace redditJsonTool;

public static class FfmpegHelper
{
    public static async Task SpeedUpMp3FileAsync(byte[] inputMp3Bytes, string outputFilePath, double speedFactor)
    {
        //save temp file to read from
        var tempInputPath = "temp_input.mp3";
        await File.WriteAllBytesAsync(tempInputPath, inputMp3Bytes);
        
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "./ffmpeg/ffmpeg.exe",
                Arguments = $"-i {tempInputPath} -filter:a \"atempo={speedFactor}\" -vn {outputFilePath} -y",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();

        
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
        {
            throw new Exception($"FFmpeg error when attempting to speed up mp3: {error}");
        }
        //delete the temp input file
        File.Delete(tempInputPath);
        
        //save new mp3 file
        Console.WriteLine("FFmpeg speed up process completed successfully.");
    }
    
    public static async Task BurnSubtitlesIntoVideoAsync(string inputVideoPath, string subtitlesPath, string outputVideoPath)
    {
        // ffmpeg -i input.mp4 -vf subtitles=subtitles.srt output.mp4
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg/ffmpeg.exe",
                Arguments = $"-i {inputVideoPath} -vf subtitles={subtitlesPath}:force_style='Alignment=0,Fontsize=15,Outline=2,MarginV=100' {outputVideoPath} -y -c:v libx264 -c:a aac -b:a 192k",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();

        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
        {
            throw new Exception($"FFmpeg error when burning subtitles onto video: {error}");
        }
        
        Console.WriteLine("FFmpeg subtitle burn-in process completed successfully.");
    }
    
    public static async Task AddAudioToVideoAsync(string inputVideoPath, string inputAudioPath, string outputVideoPath)
    {
        // ffmpeg -i input.mp4 -i input.mp3 -c:v copy -c:a aac -strict experimental output.mp4
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg/ffmpeg.exe",
                Arguments = $"-i {inputVideoPath} -i {inputAudioPath} -c:v copy -c:a aac -strict experimental -shortest {outputVideoPath} -y",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };
        process.Start();

        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        if (process.ExitCode != 0)
        {
            throw new Exception($"FFmpeg error when adding audio to video: {error}");
        }
        
        Console.WriteLine("FFmpeg add audio to video process completed successfully.");
    }
}