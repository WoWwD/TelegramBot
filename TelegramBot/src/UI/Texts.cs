using System.Collections.Generic;

namespace TelegramBot.UI
{
    public class Texts
    {
        public string GetMenu() =>
        "Список доступных команд:\n\n" +
        $"{char.ConvertFromUtf32(0x1F4DA)}  /r[группа] - Расписание группы\n\n" +
        $"{char.ConvertFromUtf32(0x1F4F0)}  /newsOfWeek - Получить новости за текущую неделю\n\n" +
        $"{char.ConvertFromUtf32(0x1F3EB)}  /links - Ссылки на социальные сети ПензГТУ\n\n";

        public string GetNews(List<string> news) =>
            $"{char.ConvertFromUtf32(0x2757)}<b>Новости недели</b>{char.ConvertFromUtf32(0x2757)}" +
            $"\n\n{string.Join("\n\n", news)}";

        public const string firstLaunch =
            "Привет!\nЧтобы начать пользоваться ботом, необходимо нажать кнопку \"Команды\", " +
            "которая появилась под полем для ввода сообщений.";

        public string Loading() => $"{char.ConvertFromUtf32(0x23F3)} Пожалуйста, подождите {char.ConvertFromUtf32(0x23F3)}";
    }
}