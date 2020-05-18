using RulesBot.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YAUL.Utilities;

namespace RulesBot
{
    public static class Program
    {
        private static string LogFile = null;
        private static readonly string[] AppDirectories = new string[]
        {
            "logs"
        };

        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }
        public static readonly DateTime Started = DateTime.UtcNow;

        public async static Task Main()
        {
            Log.LogEmitted += Log_LogEmitted;
            Log.Info("RulesBot started, press ^C to exit");
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DIHost.Setup();

            var mre = new ManualResetEvent(false);
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                mre.Set();
            };

            ConfigurationHost.Initialize();

            EnsureDirectories();

            LogFile = PathUtils.MakeAbsolute("logs", $"log_{Program.Started:yyyyMMddHHmmss}.log");


            var bot = DIHost.Get<DiscordBot>();
            await bot.Start();

            mre.WaitOne();

            bot.Dispose();

            Log.Info("RulesBot exited gracefully");
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Exception(e.ExceptionObject as Exception);
        }

        private static void EnsureDirectories()
        {
            foreach (var dir in AppDirectories)
                Directory.CreateDirectory(PathUtils.MakeAbsolute(dir));
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
