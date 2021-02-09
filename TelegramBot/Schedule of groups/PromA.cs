using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using TelegramBot.All_Paths;

namespace TelegramBot.Schedule_of_groups
{
    class PromA: IDisposable
    {
        private List<string> FormsOfEducationPromA = new List<string>();
        private string linkPromA;
        private string linkCoursPromA;
        public static string linkGroupPromA;
        public bool GetPromA(Update update)
        {
            string url, nameGroup, potok, yearInStr;
            var msg = update.Message.Text;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Paths.ulevelHigh);
            foreach (HtmlNode s in v.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                url = s.OuterHtml.Split('=').Last().Split('>')[0];
                url= Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                FormsOfEducationPromA.Add(url);
            }
            HtmlDocument m = aa.Load(Paths.ulevelMid);
            foreach (HtmlNode s in m.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                url = s.OuterHtml.Split('=').Last().Split('>')[0];
                url = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                FormsOfEducationPromA.Add(url);
            }
            for (int i = 0; i < FormsOfEducationPromA.Count; i++)
            {
                HtmlDocument formOfEducation = aa.Load(FormsOfEducationPromA[i].ToString());
                foreach (HtmlNode p in formOfEducation.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                {
                    if (p.OuterHtml.Contains("Промежуточная аттестация"))
                    {
                        url = p.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                        linkPromA = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                        HtmlDocument pa = aa.Load(linkPromA);
                        if (pa.DocumentNode.SelectNodes(Paths.tForParsRaspPromA) == null)
                        {
                            break;
                        }
                        foreach (HtmlNode c in pa.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                        {
                            potok = c.OuterHtml.ToUpper().Split('>')[1].Split('<')[0].Substring(0, 2);
                            yearInStr = msg.ToUpper().Split('R')[1].Substring(0, 2);
                            if (potok == yearInStr)
                            {
                                url = c.OuterHtml.Split('=').Last().Split('>')[0];
                                linkCoursPromA = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                                HtmlDocument cours = aa.Load(linkCoursPromA);
                                foreach (HtmlNode g in cours.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                                {
                                    nameGroup = g.OuterHtml.ToUpper().Split('>')[1].Split('<')[0];
                                    linkGroupPromA = g.OuterHtml.Split('=').Last().Split('>')[0];
                                    linkGroupPromA = Paths.uTTVuz + linkGroupPromA.Substring(2, linkGroupPromA.Length - 4) + "";
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
        public static bool GetResPromA(Update update)
        {
            using (PromA p = new PromA())
            {
                var res = p.GetPromA(update);
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