﻿using Model.Properties;
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
            WebStore.CheckStoredPath();
        }
        public static String WebStore
        {
            get;
            private set;
        } = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), AppSettings.Default.StoreRoot);

        public static String DailyStorePath(this DateTime date)
        {
            String dailyPath = "".GetDateStylePath(date);
            Path.Combine(WebStore, dailyPath).CheckStoredPath();
            return Path.Combine(AppSettings.Default.StoreRoot, dailyPath);
        }

        public static String PrefixStorePath(this String prefix)
        {
            return Path.Combine(WebStore, prefix).CheckStoredPath();
        }


        public static String DailyStorePath(this DateTime date, out string absolutePath)
        {
            String dailyPath = "".GetDateStylePath(date);
            absolutePath = Path.Combine(WebStore, dailyPath);
            absolutePath.CheckStoredPath();
            return Path.Combine(AppSettings.Default.StoreRoot, dailyPath);
        }

        public static String DailyStorePath(this DateTime date, String fileName, out string absolutePath)
        {
            String dailyPath = "".GetDateStylePath(date);
            String path = Path.Combine(WebStore, dailyPath);
            path.CheckStoredPath();
            absolutePath = Path.Combine(path, fileName);
            return Path.Combine(AppSettings.Default.StoreRoot, dailyPath, fileName);
        }

        public static String StoreTargetPath(this String storePath)
        {
            return Path.Combine(Model.Properties.AppSettings.AppRoot, storePath);
        }
    }
}
