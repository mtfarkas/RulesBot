using System.IO;

namespace RulesBot.Core.Utils
{
    public static class FileUtils
    {
        public static string AppDir { get => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); }

        public static string MakeAbsolute(string path) => Path.Join(AppDir, path);
        public static string MakeAbsolute(params string[] paths) => Path.Join(AppDir, Path.Combine(paths));
    }
}
