using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WhatTheDown.Posts;

public class RedditPost : PostBase
{
    private const string PostRegEx = "(https://www.reddit.com/r/(.*)/(.*)/)";
    private const string VideoRegEx = "(https://sd.redditsave.com/download.php?(.*))";
    private const string ImageRegEx = "(https://i.redd.it/(.*))";
    private const string GifRegEx = "(d/(.*))";

    private const string ContentXPath = "/html/body/div[3]/div[2]/div[2]/div[2]/table[2]/tbody/tr/td[1]/div/a";
    private const string CaptionXPath = "/html/body/div[3]/div[2]/div[2]/h2";

    private const string DownloadProvider = "https://www.redditsave.com/info?url=";

    private static readonly Dictionary<string, PostType> RegExToType = new Dictionary<string, PostType>
    {
        {VideoRegEx, PostType.Video},
        {ImageRegEx, PostType.Image},
        {GifRegEx, PostType.GIF}
    };

    public RedditPost(string postUrl) : 
    base(postUrl, DownloadProvider, PostRegEx, ContentXPath, CaptionXPath)
    { }

    public override async Task<string> GetContentUrlAsync()
    {
        var url = await base.GetContentUrlAsync();
        _contentUrl = await GetPostTypeAsync(_contentUrl) == PostType.GIF
                        ? DownloadProvider[0..26] + _contentUrl
                        : _contentUrl;   
        return _contentUrl!;
    }

    public override async Task<PostType> GetPostTypeAsync(string? downloadUrl = null)
    {
        var contentLink = downloadUrl ?? await GetContentUrlAsync();
        foreach (var pattern in new string[] { VideoRegEx, ImageRegEx, GifRegEx })
        {
            if (new Regex(pattern).IsMatch(contentLink)) return RegExToType[pattern];
        }
        throw new Exception("Invalid URL");
    }
}