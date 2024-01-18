using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotExample
{
    public class TelegramBotClass
    {
        private string Token;
        private string channelName;
        private string postText;
        private string photoOrVideoUrl;
        private DateTime postCreationTime;
        private bool isEditingPost;

        public TelegramBotClass(string token)
        {
            this.Token = token;
        }

        public async Task BotHandle()
        {
            var botClient = new TelegramBotClient(Token);
            using CancellationTokenSource cts = new();

            botClient.StartReceiving(
                updateHandler: async (bot, update, ct) => await HandleUpdateAsync(bot, update, ct),
                pollingErrorHandler: HandlePollingErrorAsync,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }

        private bool ValidateAdditionalData()
        {
            // Additional data validation logic
            return true;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message || message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            if (messageText == "/start")
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Hello, write \"create post\" to create a post",
                    cancellationToken: cancellationToken);
            }
            else if (messageText == "create post")
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Write channel name",
                    cancellationToken: cancellationToken);
            }
            else if (messageText.StartsWith("@"))
            {
                channelName = messageText;
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Copy photo or video url",
                    cancellationToken: cancellationToken);
            }
            else if (messageText.StartsWith("https:"))
            {
                photoOrVideoUrl = messageText;
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Write Text to Post",
                    cancellationToken: cancellationToken);
            }
            else if (messageText.EndsWith("."))
            {
                postText = messageText;
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Choose",
                    replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Show Post" },
                        new KeyboardButton[] { "Posting the post to channel" },
                        new KeyboardButton[] { "Edit Post" }
                    }),
                    cancellationToken: cancellationToken);
            }
            else if (messageText == "Show Post")
            {
                var text = $"{postText} {photoOrVideoUrl}.";

                // Save the time when the post was created
                postCreationTime = DateTime.Now;

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: text,
                    cancellationToken: cancellationToken);
            }
            else if (messageText == "Edit Post")
            {
                isEditingPost = true;

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Choose",
                    replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Channel Update" },
                        new KeyboardButton[] { "Post text update" },
                        new KeyboardButton[] { "Image or Video url update" },
                        new KeyboardButton[] { "Back" }
                    }),
                    cancellationToken: cancellationToken);
            }
            else if (messageText == "Channel Update" && isEditingPost)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Write channel name",
                    replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Channel Update" },
                        new KeyboardButton[] { "Post text update" },
                        new KeyboardButton[] { "Image or Video url update" },
                        new KeyboardButton[] { "Back" }
                    }),
                    cancellationToken: cancellationToken);
            }
            else if (messageText == "Post text update" && isEditingPost)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Write text to Post",
                    replyMarkup: new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Channel Update" },
                        new KeyboardButton[] { "Post text update" },
                        new KeyboardButton[] { "Image or Video url update" },
                        new KeyboardButton[] { "Back" }
                    }),
                    cancellationToken: cancellationToken);
            }
        }

        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
