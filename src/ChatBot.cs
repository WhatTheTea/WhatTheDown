using Telegram.Bot;
using Telegram.Bot.Types;

using System.Text.Json;

namespace WhatTheDown
{
    public sealed class ChatBot
    {/*
        public ChatBot(ITelegramBotClient botClient)
        {}

        protected override Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now}] Error : {JsonSerializer.Serialize(ex.Message)}");
            return Task.CompletedTask;
        }

        protected override async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message ?? throw new ArgumentNullException("Incoming message was null");
            switch (message.Text)
            {
                case "/start":
                    await botClient.SendTextMessageAsync(message.Chat, "Привіт, я ШоЗаДаун. А ти й без ШоЗа :)");
                    break;
                default:
                    await botClient.SendTextMessageAsync(message.Chat, "Ти попутав?");
                    break;
            }
        }
        */
    }
}