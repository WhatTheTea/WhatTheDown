using Telegram.Bot;
using Telegram.Bot.Types;

namespace WhatTheDown
{
    public sealed class TikTokDownloader
    {
        private readonly HttpClient _httpClient;

        internal TikTokDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        internal async Task OnUpdate(object? sender, (ITelegramBotClient botClient, Update update) e)
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
}