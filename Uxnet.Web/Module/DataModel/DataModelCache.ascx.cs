using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Caching;

using Utility;

namespace Uxnet.Web.Module.DataModel
{
    public partial class DataModelCache : System.Web.UI.UserControl
    {

        public Object DataItem
        {
            get
            {
                return Cache[dataID];
            }
            set
            {
                if (value != null)
                {
                    String key = dataID;
                    List<String> items = Cache[Session.SessionID] as List<String>;
                    if (items == null)
                    {
                        items = new List<string>();
                        Cache.Insert(Session.SessionID, items, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(Session.Timeout));
                    }
                    if (!items.Contains(key))
                    {
                        items.Add(key);
                    }
                    Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(Session.Timeout));
                }
                else
                {
                    Cache.Remove(dataID);
                }
            }
        }

        private String dataID
        {
            get
            {
                return String.Format("{0}{1}", Session.SessionID, KeyName);
            }
        }

        public String KeyName
        {
            get;
            set;
        }

        public void Clear()
        {
            List<String> items = Cache[Session.SessionID] as List<String>;
            if (items != null && items.Count>0)
            {
                //Cache.Remove(Session.SessionID);
                foreach (var key in items)
                {
                    Cache.Remove(key);
                }
                items.Clear();
            }
        }

    }
}