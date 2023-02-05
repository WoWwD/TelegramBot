using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.All_Paths;

namespace TelegramBot.Abstracts
{
    abstract class TimeTableActions
    {
        public string GetLinkGroup(string enteredGroup, string timeTableType)
        {
            List<string> formsOfStudy = new List<string>();
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument levelHigh = htmlWeb.Load(Constants.timeTableLevelHigh);
            HtmlDocument levelMiddle = htmlWeb.Load(Constants.timeTableLevelMiddle);
            htmlWeb.OverrideEncoding = Encoding.UTF8;

            // Добавление всех форм обучения для каждого уровня образования на сайте.
            formsOfStudy.AddRange(GetFormsOfStudy(levelHigh));
            formsOfStudy.AddRange(GetFormsOfStudy(levelMiddle));
            foreach (string form in formsOfStudy)
            {
                HtmlDocument formOfStudy = htmlWeb.Load(form.ToString());
                foreach (HtmlNode htmlNode1 in formOfStudy.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                {
                    // придумать что-то с "Установочная сессия" у заочной магистратуры и аспирантуры
                    if (htmlNode1.OuterHtml.Contains(timeTableType))
                    {
                        HtmlDocument htmlDoc = htmlWeb.Load(GetLinkFormOfStudy(htmlNode1));
                        if (htmlDoc.DocumentNode.SelectNodes(Constants.parsingTimeTable) == null) break;
                        foreach (HtmlNode htmlNode2 in htmlDoc.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                        {
                            string linkCours = GetLinkCours(htmlNode2, enteredGroup);
                            if (linkCours != null)
                            {
                                HtmlDocument cours = htmlWeb.Load(linkCours);
                                string linkGroup;
                                foreach (HtmlNode htmlNode3 in cours.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                                {
                                    linkGroup = GetLinkEnteredGroup(htmlNode3, enteredGroup);
                                    if (linkGroup != null) return linkGroup;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
        public string GetLinkEnteredGroup(HtmlNode htmlNode, string enteredGroup)
        {
            string nameGroup = htmlNode.OuterHtml.ToUpper().Split('>')[1].Split('<')[0];
            if (nameGroup == enteredGroup.ToUpper().Split('R')[1])
            {
                string link = htmlNode.OuterHtml.Split('=').Last().Split('>')[0];
                return Constants.timeTableUrl + link.Substring(2, link.Length - 4) + "";
            }
            return null;
        }
        public List<string> GetFormsOfStudy(HtmlDocument levelOfStudy)
        {
            string url;
            List<string> formsOfStudy = new List<string>();
            foreach (HtmlNode htmlNode in levelOfStudy.DocumentNode.SelectNodes(Constants.parsingTimeTable))
            {
                url = htmlNode.OuterHtml.Split('=').Last().Split('>')[0];
                url = Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
                formsOfStudy.Add(url);
            }
            return formsOfStudy;
        }
        public abstract string GetLinkFormOfStudy(HtmlNode htmlNode);
        public abstract string GetLinkCours(HtmlNode htmlNode, string enteredGroup);
    }
}