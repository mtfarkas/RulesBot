using Discord;
using Discord.WebSocket;
using RulesBot.Core;
using RulesBot.Core.Data;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RulesBot
{
    public static class Program
    {
        private static string LogFile = null;
        private static readonly string[] AppDirectories = new string[]
        {
            "logs"
        };
        public async static Task Main()
        {
            DIHost.Setup();
            Log.LogEmitted += Log_LogEmitted;

            var mre = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                mre.Set();
            };

            EnsureDirectories();

            var config = DIHost.Get<AppConfig>();
            LogFile = Path.Combine("logs",$"log_{config.CurrentRun:yyyyMMddHHmmss}.log");

            Log.Info("RulesBot started, press ^C to exit");

            var bot = DIHost.Get<DiscordBot>();
            await bot.Start();

            mre.WaitOne();

            bot.Dispose();

            Log.Info("RulesBot exited gracefully");
        }

        private static void EnsureDirectories()
        {
            foreach (var dir in AppDirectories)
                Directory.CreateDirectory(dir);
        }

        private static void Log_LogEmitted(object sender, YAUL.Data.GenericEventArgs<string> e)
        {
            Console.WriteLine(e.Value);

            try
            {
                using var sw = File.AppendText(LogFile);
                sw.AutoFlush = true;
                sw.WriteLine(e.Value);
            }
            catch { }
        }
    }
}
