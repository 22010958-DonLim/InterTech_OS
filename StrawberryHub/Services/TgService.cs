using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace StrawberryHub.Services;

public class TgService 
{
    public static async Task<string> SendMessageAsync(Int64 tgID, string txtMessage)
    {
        var botClient = new TelegramBotClient("6764592655:AAG1aGea1JO_TUrwzEnQ6zQkCeQp4jPqhow");
        var me = await botClient.GetMeAsync();

        Message message = await botClient.SendTextMessageAsync(
          chatId: tgID, 
          text: txtMessage,
          parseMode: ParseMode.Markdown
        );
        return $"{me.Id} {me.FirstName} {message.Text}";
    }
}

