using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using TelegramBot.All_Paths;

namespace TelegramBot.Abstracts
{
    abstract class TimeTableActions
    {
        public string GetLinkGroup(HtmlNode htmlNode, string message)
        {
            string nameGroup = htmlNode.OuterHtml.ToUpper().Split('>')[1].Split('<')[0];
            if (nameGroup == message.ToUpper().Split('R')[1])
            {
                string link = htmlNode.OuterHtml.Split('=').Last().Split('>')[0];
                return Constants.timeTableUrl + link.Substring(2, link.Length - 4) + "";
            }
            return null;
        }
        public abstract string GetLinkFormOfEducation(HtmlNode htmlNode);
        public abstract string GetLinkCours(HtmlNode htmlNode, string message);
    }
}