namespace WhatTheDown.Posts;

public sealed class TikTokPost : PostBase
{
    private const string PostRegEx = "https://vm.tiktok.com/(.*)/";
    private const string ContentXPath = "/html/body/div/div[1]/div/div[2]/table[2]/tbody/tr/td/div/a";
    private const string CaptionXPath = "/html/body/div/div[1]/div/h2";
    private const string DownloadProvider = "https://taksave.com/info?url=";

    public TikTokPost(string postUrl) : 
    base(postUrl, DownloadProvider, PostRegEx, ContentXPath, CaptionXPath)
    { }

    public override async Task<string> GetContentUrlAsync()
    {
        var url = await base.GetContentUrlAsync();
        return DownloadProvider[0..19] + url; // https://taksave.com/<url>
    }

    public override Task<PostType> GetPostTypeAsync(string? downloadUrl = null) => Task.FromResult<PostType>(PostType.Video);
}