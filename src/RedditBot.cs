using Telegram.Bot;
using Telegram.Bot.Types;

using HtmlAgilityPack;

namespace WhatTheDown
{
    public sealed class RedditBot
    {
        private HttpClient httpClient;
        static readonly string[] ImageExtensions = { ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF" };
        internal RedditBot()
        {
            httpClient = new();
        }
        internal async Task OnUpdate(object? sender, (ITelegramBotClient botClient, Update update) e)
        {
            var message = e.update.Message;
            var text = message?.Text;
            if (message != null && text != null)
            {

                if (text.Contains("www.reddit.com"))
                {
                    var url = "https://redditsave.com/info?url=" + text;
                    var web = new HtmlWeb();
                    var page = await web.LoadFromWebAsync(url);
                    var downloadUrl = page.DocumentNode.SelectSingleNode("/html/body/div[3]/div[2]/div[2]/div[2]/table[2]/tbody/tr/td[1]/div/a")
                                          .GetAttributeValue("href", null) ?? throw new Exception("Download url was null");
                    // Check if file is an image
                    if (ImageExtensions.Contains(Path.GetExtension(downloadUrl).ToUpperInvariant()))
                    {
                        await e.botClient.SendPhotoAsync(message.Chat, new(await httpClient.GetStreamAsync(downloadUrl)));
                    }
                    else
                    {
                        await e.botClient.SendVideoAsync(message.Chat, new(await httpClient.GetStreamAsync(downloadUrl)));
                    }
                }
            }
        }
    }

    public static class RedditDownloaderExtensions
    {
        public static Bot AddRedditDownloader(this Bot bot)
        {
            var downloader = new RedditBot();
            bot.OnUpdate += downloader.OnUpdate;
            return bot;
        }
    }
}