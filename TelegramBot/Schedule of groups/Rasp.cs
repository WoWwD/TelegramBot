using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.All_Paths;

namespace TelegramBot.Schedule_of_groups
{
    class Rasp: IDisposable
    {
        private List<string> FormsOfEducationRasp = new List<string>();
        private string linkRasp;
        private string linkCoursRasp;
        public static string linkGroupRasp;
        public bool GetRasp(Update update)
        {
            string url, nameGroup;
            int NowYear = DateTime.Now.Year - 2000, coursPars, yearInStr;
            var msg = update.Message.Text;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Paths.ulevelHigh);
            foreach (HtmlNode s in v.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                url = s.OuterHtml.Split('=').Last().Split('>')[0];
                url = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                FormsOfEducationRasp.Add(url);
            }
            HtmlDocument m = aa.Load(Paths.ulevelMid);
            foreach (HtmlNode s in m.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                url = s.OuterHtml.Split('=').Last().Split('>')[0];
                url = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                FormsOfEducationRasp.Add(url);
            }
            for (int i = 0; i < FormsOfEducationRasp.Count; i++)
            {
                HtmlDocument formOfEducation = aa.Load(FormsOfEducationRasp[i].ToString());
                foreach (HtmlNode r in formOfEducation.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                {
                    if (r.OuterHtml.Contains("Расписание занятий") || r.OuterHtml.Contains("Установочная сессия"))
                    {
                        url = r.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                        linkRasp = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                        HtmlDocument ra = aa.Load(linkRasp);
                        if (ra.DocumentNode.SelectNodes(Paths.tForParsRaspPromA) == null)
                        {
                            break;
                        }
                        foreach (HtmlNode c in ra.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                        {
                            coursPars = Convert.ToInt32(c.OuterHtml.ToUpper().Split('>')[1].Split('<')[0].Substring(0, 1));
                            yearInStr = Convert.ToInt32(msg.ToUpper().Split('R')[1].Substring(0, 2));
                            if ((NowYear - yearInStr) == coursPars)
                            {
                                url = c.OuterHtml.Split('=').Last().Split('>')[0];
                                linkCoursRasp = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                                HtmlDocument cours = aa.Load(linkCoursRasp);
                                foreach (HtmlNode g in cours.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                                {
                                    nameGroup = g.OuterHtml.ToUpper().Split('>')[1].Split('<')[0];
                                    linkGroupRasp = g.OuterHtml.Split('=').Last().Split('>')[0];
                                    linkGroupRasp = Paths.uTTVuz + linkGroupRasp.Substring(2, linkGroupRasp.Length - 4) + "";
                                    if (nameGroup == msg.ToUpper().Split('R')[1])
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
        public static bool GetResRasp(Update update)
        {
            using (Rasp r = new Rasp())
            {
                var res = r.GetRasp(update);
                if (res == true)
                {
                    return true;
                }
                return false;
            }
        }
        public void Dispose()
        {
            GC.Collect();
        }
    }
}