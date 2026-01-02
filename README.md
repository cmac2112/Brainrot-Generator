# Brainrot Generator!

This simple app allows you to provide a url of a reddit post of your choosing and create a brainrot video based on its content!

### How it works
On every reddit thread, you can add .json to the end of any url and it will give you the raw json of the post. The flow is like:

1. Provide the app with api keys from OpenAi (.srt caption creation) and ElevenLabs (voiceover creation) see provided launchSettings.json below
2. Find a brainrot like video to run as your background and call it `input.mp4` and place it in the project root directory (this will change and be easier in the future)
3. start the app with a provided url as an argument `dotnet run https://www.reddit.com/r/AskReddit/comments/1pi3cju/professionals_who_enter_peoples_homes_plumbers/.json`
4. App gets the json and parses it into a RedditPost object and takes the 3 top comments
5. Feeds it to ElevenLabs to create a voiceover
4a. Optional speedup of the voiceover with ffmpeg
6. Create .srt file to allow ffmpeg to burn captions into the video
7. Burn the video with audio and captions and output it to the /Output/ directory


 ## Example launchSettings.json
 ```
{
  "profiles": {
    "MyAspNetCoreApp": {
      "commandName": "Project",
      "environmentVariables": {
        "XI_API_KEY": "",
        "OpenAI_API_KEY": ""
      }
      }
  }
}
```

