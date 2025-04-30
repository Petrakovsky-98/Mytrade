using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using System.Globalization;

class Program
{
    static float deposit = 10000; // Твій депозит
    static float risk = 1;       // Відсоток ризику

    static async Task Main()
    {
        var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN");
        var botClient = new TelegramBotClient(botToken);

        using var cts = new CancellationTokenSource();
        var receiverOptions = new ReceiverOptions { AllowedUpdates = { } };

        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken: cts.Token);
        Console.WriteLine("Bot is running...");

        await Task.Delay(-1); // Замість Console.ReadLine()
    }


    static async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var msg = update.Message.Text.ToLower();
            var chatId = update.Message.Chat.Id;

            try
            {
                var parts = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    float entry = float.Parse(parts[0].Replace(',', '.'), CultureInfo.InvariantCulture);
                    float stop = float.Parse(parts[1].Replace(',', '.'), CultureInfo.InvariantCulture);

                    float riskAmount = (deposit * risk) / 100;
                    float size = riskAmount / Math.Abs(entry - stop);
                    float positionValue = size * entry;

                    await bot.SendTextMessageAsync(chatId, $"📈 Entry: {entry}, Stop: {stop}\n💰 Position Size: {positionValue:F2}$");
                }
                else
                {
                    await bot.SendTextMessageAsync(chatId, "Надішли **2 числа** через пробіл:\n\nНаприклад: `1.2345 1.2300`");
                }
            }
            catch
            {
                await bot.SendTextMessageAsync(chatId, "⚠️ Помилка! Перевір формат чисел (використовуй крапку).");
            }
        }
    }

    static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"❌ Error: {exception.Message}");
        return Task.CompletedTask;
    }
}


