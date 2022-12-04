using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

using System.Text.RegularExpressions;

using WhatTheDown.Posts;

namespace WhatTheDown.Downloaders;

public abstract class DownloaderBase
{
    protected readonly Regex PostRegEx;
    protected readonly HttpClient _httpClient;
    protected DownloaderBase(HttpClient httpClient, Regex regex)
    {
        _httpClient = httpClient;
        PostRegEx = regex;
    }
    internal abstract Task OnUpdate(object? sender, (ITelegramBotClient botClient, Update update) e);

    protected virtual async Task DownloadFile(ITelegramBotClient botClient, Message message, IPost post)
    {
        try
        {
            Chat chat = message!.Chat;
            var downloadUrl = await post.GetContentUrlAsync();

            var caption = await post.GetCaption();
            var captionSentBy = $"\nSent by: @{message!.From!.Username}";
            var file = new InputOnlineFile(await _httpClient.GetStreamAsync(downloadUrl));

            // Delete original message
            switch (chat.Type)
            {
                case ChatType.Group or ChatType.Supergroup:
                    var me = await botClient.GetMeAsync();
                    var meMember = await botClient.GetChatMemberAsync(chat.Id, me.Id);
                    if (meMember.Status == ChatMemberStatus.Administrator)
                    {
                        await botClient.DeleteMessageAsync(message.Chat, message.MessageId);
                    }
                    caption += captionSentBy;
                    break;

                case ChatType.Private or ChatType.Sender:
                    await botClient.DeleteMessageAsync(message.Chat, message.MessageId);
                    break;

                default:
                    //? Channels
                    break;
            }
            // Send media
            if (await post.GetPostTypeAsync() == PostType.Image)
            {
                await botClient.SendPhotoAsync(chat, file, caption: caption);
            }
            else
            {
                await botClient.SendVideoAsync(chat, file, caption: caption);
            }
        }
        catch (Exception ex)
        {
            if(ex.Message == "Bad Request: message can't be deleted") // cant delete message 
            {
                await botClient.SendTextMessageAsync(message.Chat, "Вибачте, сталася помилка :(\nВи назначили бота адміністратором, але не надали право видаляти повідомлення");
                return;
            }
            Console.WriteLine(ex.Message + " " + ex.StackTrace);
            await botClient.SendTextMessageAsync(message.Chat, "Вибачте, сталася помилка :(\nСтукніть його палицею: @WhatTheTea");
        }

    }
}
