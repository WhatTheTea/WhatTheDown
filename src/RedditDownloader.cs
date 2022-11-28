using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

using HtmlAgilityPack;

using WhatTheDown.Reddit;

namespace WhatTheDown
{
    public sealed class RedditDownloader
    {
        private readonly System.Text.RegularExpressions.Regex PostRegEx = new(@"https:\/\/www.reddit.com\/r\/(.*)\/(.*)\/");
        internal RedditDownloader()
        {
        }
        internal async Task OnUpdate(object? sender, (ITelegramBotClient botClient, Update update) e)
        {
            await DownloadFile(e.botClient, e.update);
        }

        private async Task DownloadFile(ITelegramBotClient botClient, Update update)
        {
            try
            {
                var message = update.Message;
                var match = PostRegEx.Match(message?.Text ?? string.Empty).Value; // find url in message
                if (!string.IsNullOrEmpty(match))
                {
                    Chat chat = message!.Chat;
                    var post = new RedditPost(match);
                    var downloadUrl = await post.GetContentUrlAsync();
                    var caption = await post.GetCaption();
                    var file = new InputOnlineFile(await new HttpClient().GetStreamAsync(downloadUrl));
                    // TODO: Remove httpclient from here!
                    if(await post.GetRedditPostTypeAsync() == RedditPostType.Image)
                    {
                        await botClient.SendPhotoAsync(chat, file, caption: caption);
                    } else 
                    {
                        await botClient.SendVideoAsync(chat, file, caption: caption);
                    }
                    var admins = await botClient.GetChatAdministratorsAsync(chat.Id);
                    var me = await botClient.GetMeAsync();
                    if(admins.Any(member => member.User == me)) await botClient.DeleteMessageAsync(message.Chat, message.MessageId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
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