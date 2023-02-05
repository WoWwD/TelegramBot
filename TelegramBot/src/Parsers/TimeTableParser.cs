using HtmlAgilityPack;
using System;
using System.Linq;
using TelegramBot.Abstracts;
using TelegramBot.All_Paths;

namespace TelegramBot.Parsers
{
    class TimeTableParser: TimeTableActions
    {
        public override string GetLinkFormOfStudy(HtmlNode htmlNode)
        {
            string url = htmlNode.OuterHtml.ToString().Split('=').Last().Split('>')[0];
            return Constants.timeTableUrl + htmlNode.OuterHtml.ToString().Split('=').Last().Split('>')[0].Substring(2, url.Length - 4) + "";
        }
        public override string GetLinkCours(HtmlNode htmlNode, string enteredGroup)
        {
            int currentYear = DateTime.Now.Year - 1999;
            int coursPars = Convert.ToInt32(htmlNode.OuterHtml.ToUpper().Split('>')[1].Split('<')[0].Substring(0, 1));
            int coursInMessage = Convert.ToInt32(enteredGroup.ToUpper().Split('R')[1].Substring(0, 2));
            if ((currentYear - coursInMessage) == coursPars)
            {
                string url = htmlNode.OuterHtml.Split('=').Last().Split('>')[0];
                return Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
            }
            return null;
        }
    }
}