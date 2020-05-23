using Newtonsoft.Json;
using RulesBot.Core.Data;
using System;
using System.IO;
using System.Text;
using YAUL.Data;
using YAUL.Utilities;

namespace RulesBot.Core
{
    public static class ConfigurationHost
    {
        private static readonly FileSystemWatcher fsWatcher = new FileSystemWatcher();
        public static AppConfig Current { get; private set; } = new AppConfig();
        public static event EventHandler<GenericEventArgs<AppConfig>> ConfigurationChanged;
        public static void Initialize()
        {
            fsWatcher.Path = PathUtils.MakeAbsolute("Assets", "config");
            if(Program.IsDebug) fsWatcher.Filters.Add("config.development.json");
            else fsWatcher.Filters.Add("config.json");
            fsWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fsWatcher.Changed += OnConfigurationChange;

            ReadConfiguration();

            fsWatcher.EnableRaisingEvents = true;
        }

        private static void ReadConfiguration()
        {
            Log.Info("Reading configuration");
            string contents;
            try
            {
                using var fileStream = File.Open(PathUtils.MakeAbsolute("Assets", "config", "config.json"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
                contents = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw;
            }

            JsonConvert.PopulateObject(contents, Current, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
            //JsonUtils.Populate(Current, contents);
            Log.Info("Configuration read successfully");
        }

        private static void OnConfigurationChange(object sender, FileSystemEventArgs e)
        {
            Log.Info("Configuration changed on disk");
            fsWatcher.EnableRaisingEvents = false;
            ReadConfiguration();

            ConfigurationChanged?.Invoke(null, new GenericEventArgs<AppConfig>(Current));

            fsWatcher.EnableRaisingEvents = true;
        }
    }
}
