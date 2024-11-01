using System.Collections.Specialized;
using System.Configuration;

namespace Model
{
    public class BasicConfiguration
    {
        protected static BasicConfiguration _configuration = new BasicConfiguration();
        protected NameValueCollection _values;

        protected BasicConfiguration() => this.initializeNameValueSettings();

        private void initializeNameValueSettings() => this._values = ConfigurationManager.AppSettings;

        public static BasicConfiguration Values => BasicConfiguration._configuration;

        public string this[int index] => this._values[index];

        public string this[string name] => this._values[name];
    }
}