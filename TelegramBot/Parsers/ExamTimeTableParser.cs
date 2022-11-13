using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.Abstracts;
using TelegramBot.All_Paths;

namespace TelegramBot.Schedule_of_groups
{
    class ExamTimeTableParser: TimeTableActions
    {
        private List<string> formsOfStudy = new List<string>();
        public static string linkGroupPromA;
        public bool GetPromA(Update update)
        {
            var msg = update.Message.Text;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Constants.timeTableLevelHigh);
            AddFormsOfEducation(formsOfStudy, v);
            HtmlDocument m = aa.Load(Constants.timeTableLevelMiddle);
            AddFormsOfEducation(formsOfStudy, m);
            for (int i = 0; i < formsOfStudy.Count; i++)
            {
                HtmlDocument formOfEducation = aa.Load(formsOfStudy[i].ToString());
                foreach (HtmlNode p in formOfEducation.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                {
                    if (p.OuterHtml.Contains("Промежуточная аттестация"))
                    {
                        var linkFormOfEducation = GetLinkFormOfEducation(p);
                        HtmlDocument pa = aa.Load(linkFormOfEducation);
                        if (pa.DocumentNode.SelectNodes(Constants.parsingTimeTable) == null)
                        {
                            break;
                        }
                        foreach (HtmlNode c in pa.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                        {
                            var linkCours = GetLinkCours(c, msg);
                            if (linkCours != null)
                            {
                                HtmlDocument cours = aa.Load(linkCours);
                                foreach (HtmlNode g in cours.DocumentNode.SelectNodes(Constants.parsingTimeTable))
                                {
                                    linkGroupPromA = GetLinkGroup(g, msg);
                                    if (linkGroupPromA != null)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static bool GetResPromA(Update update)
        {
            ExamTimeTableParser p = new ExamTimeTableParser();
            var res = p.GetPromA(update);
            if (res == true)
            {
                return true;
            }
            return false;
        }
        public override string GetLinkFormOfEducation(HtmlNode p)
        {
            string url = p.OuterHtml.ToString().Split('=').Last().Split('>')[0];
            return Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
        }
        public override string GetLinkCours(HtmlNode c, string message)
        {
            string potok = c.OuterHtml.ToUpper().Split('>')[1].Split('<')[0].Substring(0, 2);
            string yearInStr = message.ToUpper().Split('R')[1].Substring(0, 2);
            if (potok == yearInStr)
            {
                string url = c.OuterHtml.Split('=').Last().Split('>')[0];
                return Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
            }
            return null;
        }
        private void AddFormsOfEducation(List<string> array, HtmlDocument levelOfEducation)
        {
            string url;
            foreach (HtmlNode s in levelOfEducation.DocumentNode.SelectNodes(Constants.parsingTimeTable))
            {
                url = s.OuterHtml.Split('=').Last().Split('>')[0];
                url = Constants.timeTableUrl + url.Substring(2, url.Length - 4) + "";
                array.Add(url);
            }
        } 
    }
}