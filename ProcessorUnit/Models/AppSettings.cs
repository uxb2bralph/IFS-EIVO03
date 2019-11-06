using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace ProcessorUnit.Models
{
    public class AppSettings
    {
        public Guid InstanceID { get; set; } = Guid.NewGuid();
        public int ProcessorID { get; set; }
        public String ResponsePath { get; set; }
    }

    public static class SettingsHelper
    {
        private static AppSettings __instance;
        private static String __AppSettingsFile;

        static SettingsHelper()
        {
            __AppSettingsFile = Path.Combine(Logger.LogPath, "AppSettings.json");  //Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "AppSettings.json");
            if (__instance == null)
            {
                if (File.Exists(__AppSettingsFile))
                {
                    try
                    {
                        __instance = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(__AppSettingsFile));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }

            if (__instance == null)
            {
                __instance = new AppSettings
                {
                };
            }

            __instance.ResponsePath = __instance.ResponsePath.GetEfficientString();
        }

        public static AppSettings Instance
        {
            get
            {
                return __instance;
            }
        }

        public static String Save(this AppSettings instance)
        {
            String jsonContent = null;
            try
            {
                jsonContent = JsonConvert.SerializeObject(instance);
                File.WriteAllText(__AppSettingsFile, jsonContent);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return jsonContent;
        }
    }

}
