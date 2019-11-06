using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.Common
{
    public partial class ActionHandler : System.Web.UI.UserControl , IPostBackEventHandler
    {
        public Action<String> DoAction;

        protected bool _eventRaised;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += ActionHandler_PreRender;
        }

        void ActionHandler_PreRender(object sender, EventArgs e)
        {
            if(!_eventRaised && this.UniqueID==Request.Form["__EVENTTARGET"])
            {
                _eventRaised = true;
                RaisePostBackEvent(Request.Form["__EVENTARGUMENT"]);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (DoAction != null)
            {
                DoAction(eventArgument);
            }
            _eventRaised = true;
        }

        public String GetPostBackEventReference(String eventArgument)
        {
            return String.Format("{0};return false;", Page.ClientScript.GetPostBackEventReference(this, eventArgument));
        }

        public String GetConfirmedPostBackEventReference(String message, String eventArgumemt)
        {
            return String.Format("if(confirm(\"{0}\")) {{{1};}}; return false; ", message, GetPostBackEventReference(eventArgumemt));
        }

        public String GetConfirmedPostBackEventReference(Func<String> msgProc, String eventArgumemt)
        {
            return String.Format("if(confirm(\"{0}\")) {{{1};}}; return false; ", msgProc, GetPostBackEventReference(eventArgumemt));
        }

    }
}