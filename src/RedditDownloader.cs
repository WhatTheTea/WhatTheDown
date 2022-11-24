using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

using HtmlAgilityPack;

namespace WhatTheDown
{
    public sealed class RedditDownloader
    {
        static readonly string[] ImageExtensions = { ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF" };
        const string DownloadProvider = "https://redditsave.com/info?url=";
        internal RedditDownloader()
        {
        }
        internal async Task OnUpdate(object? sender, (ITelegramBotClient botClient, Update update) e)
        {
            var botClient = e.botClient;
            var message = e.update.Message;
            if (message != null && message.Text != null)
            {

                var downloadUrl = await GetDownloadUrl(DownloadProvider, message.Text);
                var caption = await GetPostCaption(DownloadProvider, message.Text);
                caption += $"\n Sent by: {message.SenderChat.FirstName} {message.SenderChat.LastName}";
                await SendFileToChat(message.Chat, botClient, downloadUrl, caption);
                await botClient.DeleteMessageAsync(message.Chat, message.MessageId);
            }
        }


        private async Task<string> GetDownloadUrl(string downloadProvider, string postUrl)
        {
            var url = downloadProvider + postUrl;
            var page = await new HtmlWeb().LoadFromWebAsync(url);
            var downloadUrl = page.DocumentNode.SelectSingleNode("/html/body/div[3]/div[2]/div[2]/div[2]/table[2]/tbody/tr/td[1]/div/a/@href").InnerText;
            return downloadUrl;
        }
        private async Task<string> GetPostCaption(string downloadProvider, string postUrl)
        {
            var url = downloadProvider + postUrl;
            var page = await new HtmlWeb().LoadFromWebAsync(url);
            var caption = page.DocumentNode.SelectSingleNode("/html/body/div[3]/div[2]/div[2]/h2").InnerText;
            return caption;
        }
        private async Task SendFileToChat(Chat chat, ITelegramBotClient botClient, string downloadUrl, string caption = "")
        {
            var file = new InputOnlineFile(downloadUrl);
            // Check if file is an image
            if (ImageExtensions.Contains(Path.GetExtension(downloadUrl).ToUpperInvariant()))
            {
                await botClient.SendPhotoAsync(chat, file, caption: caption);
            }
            else
            {
                await botClient.SendVideoAsync(chat, file, caption: caption);
            }
        }
    }


    public static class RedditDownloaderExtensions
    {
        public static Bot AddRedditDownloader(this Bot bot)
        {
            var downloader = new RedditDownloader();
            bot.OnUpdate += downloader.OnUpdate;
            return bot;
        }
    }
}