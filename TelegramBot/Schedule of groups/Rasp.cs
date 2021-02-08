using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;

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
            string urlFrm;
            HtmlWeb aa = new HtmlWeb();
            aa.OverrideEncoding = Encoding.UTF8;
            HtmlDocument v = aa.Load(Paths.ulevelHigh);
            foreach (HtmlNode s in v.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                urlFrm = s.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                urlFrm = Paths.uTTVuz + urlFrm.Substring(2, urlFrm.Length - 4) + "";
                FormsOfEducationRasp.Add(urlFrm);
            }
            HtmlDocument m = aa.Load(Paths.ulevelMid);
            foreach (HtmlNode s in m.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
            {
                urlFrm = s.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                urlFrm = Paths.uTTVuz + urlFrm.Substring(2, urlFrm.Length - 4) + "";
                FormsOfEducationRasp.Add(urlFrm);
            }
            int NowYear = DateTime.Now.Year - 2000;
            var msg = update.Message.Text;
            string url, nameGroup;
            int cours, yearInStr;
            HtmlWeb html = new HtmlWeb();
            html.OverrideEncoding = Encoding.UTF8;
            for (int i = 0; i < FormsOfEducationRasp.Count; i++)
            {
                HtmlDocument formOfEducation = html.Load(FormsOfEducationRasp[i].ToString());
                foreach (HtmlNode li1 in formOfEducation.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                {
                    if (li1.OuterHtml.Contains("Расписание занятий") || li1.OuterHtml.Contains("Установочная сессия"))
                    {
                        url = li1.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                        linkRasp = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                        HtmlDocument ra = html.Load(linkRasp);
                        if (ra.DocumentNode.SelectNodes(Paths.tForParsRaspPromA) == null)
                        {
                            break;
                        }
                        foreach (HtmlNode li2 in ra.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                        {
                            cours = Convert.ToInt32(li2.OuterHtml.ToString().ToUpper().Split('>')[1].Split('<')[0].Substring(0, 1));
                            yearInStr = Convert.ToInt32(msg.ToUpper().Split('R')[1].Substring(0, 2));
                            if ((NowYear - yearInStr) == cours)
                            {
                                url = li2.OuterHtml.ToString().Split('=').Last().Split('>')[0];
                                linkCoursRasp = Paths.uTTVuz + url.Substring(2, url.Length - 4) + "";
                                HtmlDocument courss = html.Load(linkCoursRasp);
                                foreach (HtmlNode li in courss.DocumentNode.SelectNodes(Paths.tForParsRaspPromA))
                                {
                                    nameGroup = li.OuterHtml.ToString().ToUpper().Split('>')[1].Split('<')[0];
                                    linkGroupRasp = li.OuterHtml.ToString().Split('=').Last().Split('>')[0];
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