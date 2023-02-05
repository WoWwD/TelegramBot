using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.All_Paths;

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
        public InlineKeyboardMarkup GetSocialNetworksButtons()
        {
            return new InlineKeyboardMarkup(
                new[]
                {
                    new[] {InlineKeyboardButton.WithUrl($"{char.ConvertFromUtf32(0x1F535)}  VK", Constants.vkontakteUrl)},
                    new[] {InlineKeyboardButton.WithUrl($"{char.ConvertFromUtf32(0x26AB)}  Instagram", Constants.instagramUrl)},
                    new[] {InlineKeyboardButton.WithUrl($"{char.ConvertFromUtf32(0x26AA)}  Facebook", Constants.facebookUrl)},
                    new[] { InlineKeyboardButton.WithUrl($"{char.ConvertFromUtf32(0x1F534)}  YouTube", Constants.youtubeUrl)}
                }
            );
        }
    }
}
