using Telegram.Bot;
using Telegram.Bot.Types;

using WhatTheDown.Posts;

namespace WhatTheDown.Downloaders;

public sealed class TikTokDownloader : DownloaderBase
{

    internal TikTokDownloader(HttpClient httpClient) :
    base(httpClient, new("https://vm.tiktok.com/(.*)/"))
    { }

    internal override async Task OnUpdate(object? sender, (ITelegramBotClient botClient, Update update) e)
    {
        var message = e.update.Message;
        var botClient = e.botClient;
        var match = PostRegEx.Match(message?.Text ?? string.Empty).Value; // find url in message
        if (match != string.Empty && message is not null)
        {
            var post = new TikTokPost(match);
            await base.DownloadFile(botClient, message, post);
        }
    }
}
public static class TikTokDownloaderExtensions
{
    public static Bot AddTikTokDownloader(this Bot bot, HttpClient httpClient)
    {
        var downloader = new TikTokDownloader(httpClient);
        bot.OnUpdate += downloader.OnUpdate;
        return bot;
    }
}
