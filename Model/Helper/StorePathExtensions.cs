using Model.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Model.Helper
{
    public static class StorePathExtensions
    {
        static StorePathExtensions()
        {
            StoreRoot.CheckStoredPath();
        }
        public static String StoreRoot
        {
            get;
            private set;
        } = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), Settings.Default.StoreRoot);
        public static String AppRoot
        {
            get;
            private set;
        } = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);


        public static String DailyStorePath(this DateTime date)
        {
            String dailyPath = "".GetDateStylePath(date);
            Path.Combine(StoreRoot, dailyPath).CheckStoredPath();
            return Path.Combine(Settings.Default.StoreRoot, dailyPath);
        }

        public static String DailyStorePath(this DateTime date, out string absolutePath)
        {
            String dailyPath = "".GetDateStylePath(date);
            absolutePath = Path.Combine(StoreRoot, dailyPath);
            absolutePath.CheckStoredPath();
            return Path.Combine(Settings.Default.StoreRoot, dailyPath);
        }

        public static String DailyStorePath(this DateTime date, String fileName, out string absolutePath)
        {
            String dailyPath = "".GetDateStylePath(date);
            String path = Path.Combine(StoreRoot, dailyPath);
            path.CheckStoredPath();
            absolutePath = Path.Combine(path, fileName);
            return Path.Combine(Settings.Default.StoreRoot, dailyPath, fileName);
        }

        public static String StoreTargetPath(this String storePath)
        {
            return Path.Combine(AppRoot, storePath);
        }
    }
}
