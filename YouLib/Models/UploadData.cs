#region

using System.Collections.Generic;
using YouLib.Models.Enums;

#endregion

namespace YouLib.Models;

public class UploadData
{
    /// <summary>
    ///     Specifies the informations of the file to be upload.
    /// </summary>
    /// <param name="fileName">
    ///     Video file name to upload.
    ///     <see href="https://support.google.com/youtube/troubleshooter/2888402?hl=en" />
    /// </param>
    /// <param name="title">Title of video.</param>
    /// <param name="description">Description of video.</param>
    /// <param name="category">Category of video.</param>
    /// <param name="tags">A tags array.</param>
    public UploadData(string fileName, string title, string description, CategoryType category, IEnumerable<string> tags = null)
    {
        FileName = fileName;
        Tags = tags;
        Title = title;
        Description = description;
        Category = category;
    }

    public UploadData()
    {
    }

    public string FileName { get; set; }

    public IEnumerable<string> Tags { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public CategoryType Category { get; set; }
}