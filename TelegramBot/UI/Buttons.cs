using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.UI
{
    public class Buttons
    {
        public IReplyMarkup GetMenuButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>> { new List<KeyboardButton> { new KeyboardButton("Команды") } },
                ResizeKeyboard = true
            };
        }
    }
}
