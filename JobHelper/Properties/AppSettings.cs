using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace JobHelper.Properties
{
    public class AppSettings
    {

        public static String AppRoot
        {
            get;
            private set;
        } = AppDomain.CurrentDomain.BaseDirectory;

        static JObject _Settings;

        static AppSettings()
        {
            _default = Initialize<AppSettings>(typeof(AppSettings).Namespace);
        }

        public AppSettings()
        {

        }

        protected static T Initialize<T>(String propertyName)
            where T : AppSettings, new()
        {
            T currentSettings;
            //String fileName = $"{Assembly.GetExecutingAssembly().GetName()}.settings.json";
            String fileName = "App.settings.json";
            String filePath = Path.Combine(AppRoot, fileName);
            if (File.Exists(filePath))
            {
                _Settings = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(filePath));
            }
            else
            {
                _Settings = new JObject();
            }

            //String propertyName = Assembly.GetExecutingAssembly().GetName().Name;
            if (_Settings[propertyName] != null)
            {
                currentSettings = _Settings[propertyName].ToObject<T>();
            }
            else
            {
                currentSettings = new T();
                _Settings[propertyName] = JObject.FromObject(currentSettings);
            }

            File.WriteAllText(filePath, JsonConvert.SerializeObject(_Settings));
            return currentSettings;
        }

        static AppSettings _default;

        public static AppSettings Default
        {
            get => _default;
        }

        public String CommandDecryptGPG { get; set; } = "call pgp_decrypt.bat {0} {1}";
        public String Workspace { get; set; } = Path.Combine(AppRoot, "Workspace").CheckStoredPath();
        public String ResponseReady { get; set; } = Path.Combine(AppRoot, "ResponseReady").CheckStoredPath();
        public String InvoiceTransactionRoot { get; set; } = "C:\\Users\\10090557\\sftp";
        public String[] RequestType { get; set; } = { "SellerInvoice", "CancelInvoice", "Allowance" };
        public String CommandEncryptGPG { get; set; } = "call pgp_encrypt.bat {0} {1}";

    }

}
