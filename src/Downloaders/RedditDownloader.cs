using Telegram.Bot;
using Telegram.Bot.Types;

using WhatTheDown.Posts;

namespace WhatTheDown.Downloaders;

public sealed class RedditDownloader : DownloaderBase
{
    internal RedditDownloader(HttpClient httpClient) :
    base(httpClient, new("https://www.reddit.com/r/(.*)/(.*)/"))
    { }
    internal override async Task OnUpdate(object? sender, (ITelegramBotClient botClient, Update update) e)
    {
        var message = e.update.Message;
        var botClient = e.botClient;
        var match = PostRegEx.Match(message?.Text ?? string.Empty).Value; // find url in message
        if (match != string.Empty && message is not null)
        {
            var post = new RedditPost(match);
            await base.DownloadFile(botClient, message, post);
        }
    }
}

public static class RedditDownloaderExtensions
{
    public static Bot AddRedditDownloader(this Bot bot, HttpClient httpClient)
    {
        var downloader = new RedditDownloader(httpClient);
        bot.OnUpdate += downloader.OnUpdate;
        return bot;
    }
}