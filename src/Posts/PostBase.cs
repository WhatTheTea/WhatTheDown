using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace WhatTheDown.Posts;

public abstract class PostBase : IPost
{
    public string PostUrl { get; init; }
    private string _postRegEx;
    private string _contentXPath;
    private string _captionXPath;
    protected string? _caption;
    protected string? _contentUrl;

    protected HtmlDocument? _downloadPage;
    protected readonly string _downloadPageUrl;


    public PostBase(string postUrl, string downloadProviderUrl, string postRegEx, string contentXPath, string captionXPath)
    {
        _postRegEx = postRegEx;
        _contentXPath = contentXPath;
        _captionXPath = captionXPath;
        if (new Regex(_postRegEx).IsMatch(postUrl))
        {
            PostUrl = postUrl;
            _downloadPageUrl = downloadProviderUrl + PostUrl;
        }
        else
        {
            throw new Exception("Post url is not valid");
        }
    }

    public virtual async Task<string> GetContentUrlAsync()
    {
        if (_contentUrl == null)
        {
            _downloadPage = _downloadPage ?? await new HtmlWeb().LoadFromWebAsync(_downloadPageUrl);
            _contentUrl = _downloadPage.DocumentNode.SelectSingleNode(_contentXPath)
                                       .GetAttributeValue("href", string.Empty);
        }
        return _contentUrl;
    }

    public abstract Task<PostType> GetPostTypeAsync(string? downloadUrl = null);

    public virtual async Task<string> GetCaption()
    {
        if (_caption == null)
        {
            _downloadPage = _downloadPage ?? await new HtmlWeb().LoadFromWebAsync(_downloadPageUrl);
            _caption = _downloadPage.DocumentNode.SelectSingleNode(_captionXPath)
                                    .InnerText;
        }
        return _caption;
    }

}