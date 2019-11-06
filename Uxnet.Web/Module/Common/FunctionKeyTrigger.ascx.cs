using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace Uxnet.Web.Module.Common
{
    public partial class FunctionKeyTrigger : System.Web.UI.UserControl
    {

        private Dictionary<int, object> _eventFire = new Dictionary<int, object>();
        private Dictionary<int, object> _altHotKeyFire = new Dictionary<int, object>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(FunctionKeyTrigger), "initFunc"))
            {
                registerInitFunctionKeyEnv();
            }
        }

        public void AddFunctionKeyFire(int keyCode, Control target)
        {
            if(_eventFire.ContainsKey(keyCode))
            {
                _eventFire[keyCode] = target;
            }
            else
            {
                _eventFire.Add(keyCode,target);
            }
        }

        public void AddFunctionKeyFire(int keyCode, string target)
        {
            if (_eventFire.ContainsKey(keyCode))
            {
                _eventFire[keyCode] = target;
            }
            else
            {
                _eventFire.Add(keyCode, target);
            }
        }


        public void RemoveFunctionKeyFire(int keyCode)
        {
            _eventFire.Remove(keyCode);
        }


        public void AddAltHotKeyFire(int keyCode, Control target)
        {
            if (_altHotKeyFire.ContainsKey(keyCode))
            {
                _altHotKeyFire[keyCode] = target;
            }
            else
            {
                _altHotKeyFire.Add(keyCode, target);
            }
        }

        public void AddAltHotKeyFire(int keyCode, string target)
        {
            if (_altHotKeyFire.ContainsKey(keyCode))
            {
                _altHotKeyFire[keyCode] = target;
            }
            else
            {
                _altHotKeyFire.Add(keyCode, target);
            }
        }


        public void RemoveAltHotKeyFire(int keyCode)
        {
            _altHotKeyFire.Remove(keyCode);
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(FunctionKeyTrigger_PreRender);
        }

        void FunctionKeyTrigger_PreRender(object sender, EventArgs e)
        {
            buildFireEventHandler();
            buildAltHotKeyHandler();
        }

        private void buildFireEventHandler()
        {
            if (_eventFire.Count > 0)
            {
                foreach (var fire in _eventFire)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("handler = new Object();\r\n")
                        .Append("handler.keyCode = ").Append(fire.Key).Append(";\r\n")
                        .Append("handler.handler = fireTarget").Append(fire.Key).Append(";\r\n")
                        .Append("__functionkeyHandler[__functionkeyHandler.length] = handler;\r\n");
                    Page.ClientScript.RegisterClientScriptBlock(typeof(FunctionKeyTrigger), String.Format("fireTarget{0}", fire.Key), sb.ToString(), true);
                    sb.Remove(0, sb.Length);

                    if (fire.Value is Control)
                    {
                        sb.Append("    function fireTarget").Append(fire.Key).Append("() {\r\n")
                            .Append(Page.ClientScript.GetPostBackEventReference((Control)fire.Value, ""))
                            .Append(";\r\n return true; \r\n}\r\n");
                        this.Page.ClientScript.RegisterClientScriptBlock(typeof(FunctionKeyTrigger), ((Control)fire.Value).UniqueID,
                            sb.ToString(), true);
                    }
                    else
                    {
                        sb.Append("    function fireTarget").Append(fire.Key).Append("() {\r\n")
                            .Append(fire.Value)
                            .Append("\r\n return true; \r\n}\r\n");
                        this.Page.ClientScript.RegisterClientScriptBlock(typeof(FunctionKeyTrigger), String.Format("firebody{0}", fire.Key),
                            sb.ToString(), true);
                    }

                }
            }
        }

        private void buildAltHotKeyHandler()
        {
            if (_altHotKeyFire.Count > 0)
            {
                foreach (var fire in _altHotKeyFire)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("handler = new Object();\r\n")
                        .Append("handler.keyCode = ").Append(fire.Key).Append(";\r\n")
                        .Append("handler.handler = fireTarget").Append(fire.Key).Append(";\r\n")
                        .Append("__functionkeyHandler[__functionkeyHandler.length] = handler;\r\n");
                    Page.ClientScript.RegisterClientScriptBlock(typeof(FunctionKeyTrigger), String.Format("fireTarget{0}", fire.Key), sb.ToString(), true);
                    sb.Remove(0, sb.Length);

                    if (fire.Value is Control)
                    {
                        sb.Append("    function fireTarget").Append(fire.Key).Append(@"() { 
                                       if(event.altKey) { 
                                ").Append(Page.ClientScript.GetPostBackEventReference((Control)fire.Value, ""))
                            .Append(@";
                                    return true; 
                                } else {
                                    return false;
                                }
                            }
                            ");
                        this.Page.ClientScript.RegisterClientScriptBlock(typeof(FunctionKeyTrigger), ((Control)fire.Value).UniqueID,
                            sb.ToString(), true);
                    }
                    else
                    {

                        sb.Append("    function fireTarget").Append(fire.Key).Append(@"() { 
                                       if(event.altKey) { 
                                ").Append(fire.Value)
                            .Append(@"
                                    return true; 
                                } else {
                                    return false;
                                }
                            }
                            ");
                        this.Page.ClientScript.RegisterClientScriptBlock(typeof(FunctionKeyTrigger), String.Format("firebody{0}", fire.Key),
                            sb.ToString(), true);
                    }

                }
            }
        }




        private void registerInitFunctionKeyEnv()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" var __functionkeyHandler = new Array();\r\n")
                .Append(@"
                    function shutDown(evt) {
                            evt = (evt) ? evt : ((event) ? event : null);
                            if (evt) {
                                for (i = 0; i < __functionkeyHandler.length; i++) {
                                    if (__functionkeyHandler[i].keyCode == evt.keyCode) {
                                       if(__functionkeyHandler[i].handler()) {
                                            evt.returnValue = false;
                                        }
                                    }
                                }
                            }
                        }").Append("\r\n")
                .Append("document.onkeydown = shutDown;\r\n")
                .Append("document.onhelp = returnFalse;\r\n")
                .Append(@"
                        function returnFalse() {
                            return false; 
                        }");

            Page.ClientScript.RegisterClientScriptBlock(
                typeof(FunctionKeyTrigger), "initFunc",sb.ToString(),true);
        }
    }
}