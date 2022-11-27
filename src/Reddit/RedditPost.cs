using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WhatTheDown.Reddit;

public class RedditPost
{
    private const string PostRegEx = @"(https:\/\/www.reddit.com\/r\/(.*)\/(.*)\/)";
    private const string VideoRegEx = @"(https:\/\/sd.redditsave.com\/download.php?(.*))";
    private const string ImageRegEx = @"(https:\/\/i.redd.it\/(.*))";
    private const string GifRegEx = @"(d/\(.*))";

    private const string linkXPath = "/html/body/div[3]/div[2]/div[2]/div[2]/table[2]/tbody/tr/td[1]/div/a";
    private const string captionXPath = "/html/body/div[3]/div[2]/div[2]/h2";

    private static readonly Dictionary<string, RedditPostType> RegExToType = new Dictionary<string, RedditPostType>
    {
        {VideoRegEx, RedditPostType.Video},
        {ImageRegEx, RedditPostType.Image},
        {GifRegEx, RedditPostType.GIF}
    };

    private HtmlDocument? _downloadPage;
    private readonly string _postUrl;
    private string? _caption;
    private string? _contentUrl;

    public string PostUrl => _postUrl;

    public RedditPost(string url)
    {
        if (new Regex(PostRegEx).IsMatch(url))
        {
            _postUrl = url;
        }
        else
        {
            throw new Exception("Post url is not valid");
        }
    }

    public async Task<string> GetContentUrlAsync()
    {
        if (_contentUrl == null)
        {
            _downloadPage = _downloadPage ?? await new HtmlWeb().LoadFromWebAsync(_postUrl);
            var contentLink = _downloadPage.DocumentNode.SelectSingleNode(linkXPath)
                                          .GetAttributeValue("href", string.Empty);
            _contentUrl = await GetRedditPostTypeAsync(contentLink) == RedditPostType.GIF
                        ? "https://redditsave.com/" + contentLink
                        : contentLink;
        }
        return _contentUrl;
    }

    public async Task<RedditPostType> GetRedditPostTypeAsync(string? downloadUrl = null)
    {
        var contentLink = downloadUrl ?? await GetContentUrlAsync();
        foreach (var pattern in new string[] { VideoRegEx, ImageRegEx, GifRegEx })
        {
            if (new Regex(pattern).IsMatch(contentLink)) return RegExToType[pattern];
        }
        throw new Exception("Invalid URL");
    }

    public async Task<string> GetCaption()
    {
        if (_caption == null)
        {
            _downloadPage = _downloadPage ?? await new HtmlWeb().LoadFromWebAsync(_postUrl);
            _caption = _downloadPage.DocumentNode.SelectSingleNode(captionXPath)
                                    .InnerText;
        }
        return _caption;
    }

}