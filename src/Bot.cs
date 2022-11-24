using System.Text.Json;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WhatTheDown
{
    public class Bot
    {
        private ITelegramBotClient _botClient;
        public ITelegramBotClient BotClient => _botClient; 
        public delegate Task EventHandlerAsync<TArgs>(object sender, TArgs e);
        public event EventHandlerAsync<(ITelegramBotClient botClient, Update update)>? OnUpdate;
        public event EventHandlerAsync<(ITelegramBotClient botClient, Exception ex)>? OnError;
        public Bot(string apiKey)
        {
            _botClient = new TelegramBotClient(apiKey);

            var cancellationToken = new CancellationTokenSource().Token;
            var receiverOptions = new ReceiverOptions { AllowedUpdates = new UpdateType[] { UpdateType.Message, } };
            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
        }
        
        private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            OnUpdate?.Invoke(this,(botClient, update));
            return Task.CompletedTask;
        }
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{DateTime.Now}] Error : {JsonSerializer.Serialize(ex.Message)}");
            OnError?.Invoke(this, (botClient, ex));
            return Task.CompletedTask;
        }
    }
}