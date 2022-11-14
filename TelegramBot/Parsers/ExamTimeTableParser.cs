using HtmlAgilityPack;
using System.Linq;
using TelegramBot.Abstracts;
using TelegramBot.All_Paths;

namespace TelegramBot.Parsers
{
    class ExamTimeTableParser: TimeTableActions
    {
        public override string GetLinkFormOfStudy(HtmlNode htmlNode)
        {
            string url = htmlNode.OuterHtml.ToString().Split('=').Last().Split('>')[0];
            return Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
        }
        public override string GetLinkCours(HtmlNode htmlNode, string enteredGroup)
        {
            string potok = htmlNode.OuterHtml.ToUpper().Split('>')[1].Split('<')[0].Substring(0, 2);
            string yearInStr = enteredGroup.ToUpper().Split('R')[1].Substring(0, 2);
            if (potok == yearInStr)
            {
                string url = htmlNode.OuterHtml.Split('=').Last().Split('>')[0];
                return Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
            }
            return null;
        }
    }
}