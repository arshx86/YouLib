#region

using System;
using System.Threading;
using System.Threading.Tasks;
using YouLib.Models;
using YouLib.Models.Enums;

#endregion

namespace YouLib.Tests
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            YouClient client =
                new YouClient(
                    "",
                    "",
                    "");

            client.OnProgressed += progress => { Console.WriteLine($"Progress: {progress}"); };
            client.OnUploadFinished += url => { Console.WriteLine($"Upload finished! {url}"); };
            client.OnErrored += error => { Console.WriteLine($"Upload failed: {error.Message}"); };

            Console.WriteLine("Uploading...");
            await client.UploadVideo(new UploadData
            {
                Category = CategoryType.ScienceTech,
                Description = "nooothing",
                Title = "chillin with cs",
                Tags = new[]
                {
                    "csharp",
                    "video",
                    "youtubeapi"
                },
                FileName = "C:\\Users\\arsh\\Desktop\\videoplayback.mp4"
            });

            Thread.Sleep(-1);
        }
    }
}