using Telegram.Bot;
using Telegram.Bot.Types;

namespace WhatTheDown.Downloaders;

public sealed class TikTokDownloader : DownloaderBase
{

    internal TikTokDownloader(HttpClient httpClient) :
    base(httpClient, new(""))
    { }

    internal override async Task OnUpdate(object? sender, (ITelegramBotClient botClient, Update update) e)
    {
        
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
