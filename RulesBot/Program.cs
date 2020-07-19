using RulesBot.Core;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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
            //ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateCertificate);

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

            LogFile = PathUtils.MakeAbsolute("logs", $"log_{Program.Started:yyyy_MM_dd__HH_mm_ss}.log");


            var bot = DIHost.Get<DiscordBot>();
            await bot.Start();

            mre.WaitOne();

            bot.Dispose();

            Log.Info("RulesBot exited gracefully");
        }

        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
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
