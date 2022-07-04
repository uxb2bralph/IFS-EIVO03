using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebHome.Properties
{
    public partial class Settings : IDisposable
    {
        public static String AppRoot
        {
            get;
            private set;
        } = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

        static JObject _Settings;

        static Settings()
        {
            _default = Initialize<Settings>(typeof(Settings).Namespace);
        }

        public Settings()
        {

        }

        protected void Save()
        {
            String fileName = "App.settings.json";
            String filePath = Path.Combine(AppRoot, "App_Data", fileName);
            String propertyName = typeof(Settings).Namespace;
            _Settings[propertyName] = JObject.FromObject(this);
            File.WriteAllText(filePath, _Settings.ToString());
        }

        protected static T Initialize<T>(String propertyName)
            where T : Settings, new()
        {
            T currentSettings;
            //String fileName = $"{Assembly.GetExecutingAssembly().GetName()}.settings.json";
            String fileName = "App.settings.json";
            String filePath = Path.Combine(AppRoot, "App_Data", fileName);
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

            File.WriteAllText(filePath, _Settings.ToString());
            return currentSettings;
        }

        public void Dispose()
        {
            dispose(true);
        }

        bool _disposed;
        protected void dispose(bool disposing)
        {
            if (!_disposed)
            {
                this.Save();
            }
            _disposed = true;
        }

        ~Settings()
        {
            dispose(false);
        }

        static Settings _default;

        public static Settings Default => _default;
        public String eInvoiceConnectionString { get; set; } = "Data Source=192.168.200.23\\sqlexpress,1433;Initial Catalog=EIVO03;User ID=eivo;Password=eivoeivo;Min Pool Size=10;Max Pool Size=1000;MultipleActiveResultSets=true;";
        public string BuyerOrderPrefix { get; set; } = "30";
        public string CsvUploadEncoding { get; set; } = "Big5";
        public string DefaultUILanguage { get; set; } = "zh-TW";
        public string DefaultUserCarrierType { get; set; } = "3J0001";
        public int EIVO_Service { get; set; } = 1;
        public bool EnableGovPlatform { get; set; } = true;
        public bool EnableJobScheduler { get; set; } = true;
        public string ExceptionNotificationUrl { get; set; } = "~/Notification/DataUploadExceptionList";
        public string GenerateMemberCodeUrl { get; set; } = "~/Published/GenerateMemberCode.ashx";
        public string MailServer { get; set; } = "localhost";
        public int MaxResponseCountPerBatch { get; set; } = 1024;
        public string ReplyTo { get; set; } = "it_test@uxb2b.com";
        public string ServiceMailBox { get; set; } = "e-invoicevasc@uxb2b.com";
        public bool ShowAuthorizationNoInMail { get; set; } = true;
        public string SystemAdmin { get; set; } = "ifsadmin";
        public string TaskCenter { get; set; } = "TaskCenter";
        public string ThermalPOS { get; set; } = "0 0 162 792";
        public string WebApDomain { get; set; } = "http://localhost:2598";
        public string WebMaster { get; set; } = "系統管理員 <invoice_test@uxb2b.com>";
        public int UserTimeoutInMinutes = 20160;
        public int SessionTimeoutInMinutes = 180;
        public int ResourceMaxWidth = 300;

    }

}