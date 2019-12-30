using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;

namespace Uxnet.Com.Helper
{
    public class DynamicQueryStringParameter : DynamicObject
    {
        protected NameValueCollection _items;

        public DynamicQueryStringParameter()
        {
            _items = new NameValueCollection();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _items.AllKeys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _items[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _items[binder.Name] = value != null ? value.ToString() : null;
            // you can always set a key in the dictionary so return true
            return true;
        }

        public NameValueCollection Items
        {
            get
            {
                return _items;
            }
        }

        public String ToQueryString()
        {
            StringBuilder builder = new StringBuilder();
            foreach(var key in _items.AllKeys)
            {
                foreach(var v in _items.GetValues(key))
                {
                    builder.Append('&').Append(key).Append('=').Append(HttpUtility.UrlDecode(v));
                }
            }
            builder[0] = '?';
            return builder.ToString();
        }

    }
}