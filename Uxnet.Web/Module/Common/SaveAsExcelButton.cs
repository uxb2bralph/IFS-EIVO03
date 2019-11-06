using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.HtmlControls;

namespace Uxnet.Web.Module.Common
{
    public partial class SaveAsExcelButton : Button
    {
        private bool _bClicked;
        private List<Control> _downloadControls;

        public event EventHandler BeforeClick;

        public SaveAsExcelButton()
            : base()
        {

        }

        public List<Control> DownloadControls
        {
            get
            {
                if (_downloadControls == null)
                    _downloadControls = new List<Control>();
                return _downloadControls;
            }
        }

        public String FileName
        {
            get;
            set;
        }

        protected override void OnClick(EventArgs e)
        {
            OnBeforeClick();
            base.OnClick(e);
            _bClicked = true;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!_bClicked)
            {
                base.Render(writer);
            }
            else
            {
                saveToExcel();
            }
        }

        private void saveToExcel()
        {
            if (_downloadControls != null && _downloadControls.Count() > 0)
            {
                HttpResponse Response = this.Context.Response;
                Response.Clear();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "message/rfc822";
                Response.AddHeader("Content-Disposition", !String.IsNullOrEmpty(FileName) ? String.Format("attachment;filename={0}", FileName)
                    : String.Format("attachment;filename={0:yyyy-MM-dd}.xls", DateTime.Today));

                Page page = new Page();

                HtmlForm form = new HtmlForm();
                form.ID = "theForm";

                ((IParserAccessor)page).AddParsedSubObject(form);

                IParserAccessor parser = (IParserAccessor)form;

                foreach (var ctrl in _downloadControls)
                {
                    parser.AddParsedSubObject(ctrl);
                }

                using (StreamWriter sw = new StreamWriter(Response.OutputStream))
                {
                    using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                    {
                        foreach (var ctrl in _downloadControls)
                        {
                            ctrl.RenderControl(htw);
                        }
                        //form.RenderControl(htw);
                        htw.Flush();
                    }
                    sw.Flush();
                }
                Response.End();
            }
        }

        protected void OnBeforeClick()
        {
            if (BeforeClick != null)
            {
                BeforeClick(this, new EventArgs());
            }
        }


    }
}
