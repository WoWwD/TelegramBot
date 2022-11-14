using Freezer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBot.Parsers;
using static System.Net.WebRequestMethods;

namespace TelegramBot.Services
{
    public class ScreenshotService
    {
        public FileStream GetScreenshot(int screenWidth, int screenHeight, string screenshotName, string linkGroup)
        {
            Guid guid = Guid.NewGuid();
            var screenshotJob = ScreenshotJobBuilder
                .Create(linkGroup)
                .SetBrowserSize(screenWidth, screenHeight)
                .SetCaptureZone(CaptureZone.FullPage)
                .SetTrigger(new WindowLoadTrigger());
            System.IO.File.WriteAllBytes($"{guid}{screenshotName}.png", screenshotJob.Freeze());
            string[] findFile = Directory.GetFiles(
                AppDomain.CurrentDomain.BaseDirectory,
                $"{guid}{screenshotName}.png", 
                SearchOption.AllDirectories
            );
            return System.IO.File.OpenRead(findFile[0]);
        }
    }
}
