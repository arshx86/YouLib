# YouLib
A lightweight and asynchronous YouTube upload client

## How to use
In internal youlib uses [youtube V3 data API](https://developers.google.com/youtube/v3) which is hard to use. YouLib makes uploading video is so easy.

You'll need **oauth key** and some client informations which you can create in [google cloud](https://console.cloud.google.com/) Please watch a tutorial to get your API keys.

Initialize the client with your client & oauth keys;
```csharp
YouClient client =
            new YouClient(
               oAuthKey:      "",
               clientID:      "",
               clientSecret:  "");
```

> Since **oAuth** gate requires user's permission, you'll be asked for permission in your browser while you create client. If you grant it once, you won't need again.

Bind the notify events;
```csharp
client.OnProgressed += progress => { Console.WriteLine($"Progress: {progress}"); }; // When upload progress is increased
client.OnUploadFinished += url => { Console.WriteLine($"Upload finished! video link: {url}"); }; // When upload is finished. 
client.OnErrored += error => { Console.WriteLine($"Upload failed: {error.Message}"); }; // When upload is errored.
```

Then upload your video. 
```csharp
 await client.UploadVideo(new UploadData {
   Category = CategoryType.ScienceTech,
     Description = "nooothing",
     Title = "chillin with cs",
     Tags = new [] {
       "csharp",
       "youtubeapi"
     },
     FileName = "C:\\Users\\arsh\\Desktop\\videoplayback.mp4"
 });
```

Easy as that.
You can find a **demo** in [tests](https://github.com/arshx86/YouLib/blob/main/YouLib.Tests/Program.cs) solution
