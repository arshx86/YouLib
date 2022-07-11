#region

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using YouLib.Models;

#endregion

namespace YouLib;

public class YouClient
{
    private UserCredential _userCredential;

    /// <summary>
    ///     Initializes a <b>video</b> uploader service.
    /// </summary>
    public YouClient(string OAuthKey, string ClientID, string ClientSecret)
    {
        _ = Authorize(OAuthKey, ClientID, ClientSecret);
    }

    /// <summary>
    ///     Uploads a video.
    /// </summary>
    /// <param name="Data">Data class related of video to be uploaded.</param>
    public async Task UploadVideo(UploadData Data)
    {
        if (string.IsNullOrEmpty(_userCredential?.Token.RefreshToken)) throw new Exception("Failed to authenticate! please authenticate the app in your browser.");

        #region Initialize youtube service

        var yClient = new YouTubeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = _userCredential,
            ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
        });

        var Video = new Video
        {
            Snippet = new VideoSnippet
            {
                Title = Data.Title,
                Description = Data.Description,
                CategoryId = Convert.ToString((int)Data.Category)
            },
            Status = new VideoStatus
            {
                PrivacyStatus = "public" // private - listed
            }
        };

        if (Data.Tags.Any()) Video.Snippet.Tags = Data.Tags.ToArray();

        #endregion

        #region Upload

        TargetVideo = new FileInfo(Data.FileName);
        using (var client = new WebClient())
        {
            var buffer = client.DownloadData(Data.FileName);
            using (var stream = new MemoryStream(buffer))
            {
                var video = yClient.Videos.Insert(Video, "snippet,status", stream, "video/*");
                video.ProgressChanged += ProgressChanged;
                video.ResponseReceived += OnUploadSuccess;
                await video.UploadAsync();
            }
        }

        #endregion
    }

    /// <summary>
    ///     Disposes the client and stops all uploads.
    /// </summary>
    public async Task Dispose()
    {
        await _userCredential?.RevokeTokenAsync(CancellationToken.None)!;
    }

    #region Event Handlers

    public delegate void UploadFinished(string URL);

    /// <summary>
    ///     Fired when upload finished.
    /// </summary>
    public event UploadFinished OnUploadFinished;


    public delegate void Progressed(int Progress);

    /// <summary>
    ///     Fired when upload is progressed. Progress is in percent.
    /// </summary>
    public event Progressed OnProgressed;

    /// <summary>
    ///     Fired when upload is errored. <see cref="Exception" /> contains the info of error.
    /// </summary>
    /// <param name="Error"></param>
    public delegate void Errored(Exception Error);

    public event Errored OnErrored;

    #endregion

    #region Private

    /// <summary>
    ///     Event to receive "OK" status of upload.
    /// </summary>
    /// <param name="obj"></param>
    private void OnUploadSuccess(Video obj)
    {
        OnUploadFinished?.Invoke(string.IsNullOrEmpty(obj.Id) ? null : $"https://www.youtube.com/watch?v={obj.Id}");
    }

    private FileInfo TargetVideo;

    private int PreviousProgress;

    private void ProgressChanged(IUploadProgress obj)
    {
        switch (obj.Status)
        {
            case UploadStatus.Uploading:
                int percentage = (int)Math.Round(obj.BytesSent * 100.0D / TargetVideo.Length);
                if (PreviousProgress != percentage) // skip if progress is same
                {
                    PreviousProgress = percentage;
                    OnProgressed?.Invoke(percentage);
                }

                break;
            case UploadStatus.Failed:
                OnErrored?.Invoke(obj.Exception);
                break;
        }
    }

    /// <summary>
    ///     Authorizes the local oauth.
    /// </summary>
    private Task Authorize(string oAuth, string ID, string Secret)
    {
        try
        {
            _userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientSecret = Secret,
                    ClientId = ID
                },
                new[]
                {
                    YouTubeService.Scope.YoutubeUpload
                },
                "user",
                CancellationToken.None
            ).Result; // should be waited
            if (string.IsNullOrEmpty(_userCredential?.Token.RefreshToken)) throw new Exception("Failed to authenticate! please authenticate the app in your browser.");
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to authenticate! check credintials. Error code: {e.Message}");
        }

        return Task.CompletedTask;
    }

    #endregion
}