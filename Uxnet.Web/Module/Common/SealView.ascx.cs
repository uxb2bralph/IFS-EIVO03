using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Drawing;

namespace Uxnet.Web.Module.Common
{
    public partial class SealView : System.Web.UI.UserControl
    {
        private string _sealPath;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void BindData(string sealPath)
        {
            _sealPath = sealPath;
            this.DataBind();
        }

        public string ImageUrl
        {
            get
            {
                return _sealPath;
            }
            set
            {
                _sealPath = value;
            }
        }

        public void Attach(WebControl target, string posX, string posY,string imageUrl)
        { 
            target.Style["background-repeat"] = "no-repeat";
            target.Style["background-position"] = String.Format("{0} {1}",posX,posY);
            target.Style["background-image"] = String.Format("url({0})", imageUrl);
        }

        public void Attach(HtmlControl target, string posX, string posY, string imageUrl)
        {
            target.Style["background-repeat"] = "no-repeat";
            target.Style["background-position"] = String.Format("{0} {1}", posX, posY);
            target.Style["background-image"] = String.Format("url({0})", imageUrl);
        }



        public override void DataBind()
        {
            if (!String.IsNullOrEmpty(_sealPath))
            {
                base.DataBind();

                string sealPath = Server.MapPath(_sealPath);

                if (File.Exists(sealPath))
                {
                    using (Bitmap bmp = new Bitmap(sealPath))
                    {
                        divImg.Style.Add("height", String.Format("{0}in", (float)bmp.Height / bmp.HorizontalResolution));
                        divImg.Style.Add("width", String.Format("{0}in", (float)bmp.Width / bmp.VerticalResolution));
                        divImg.Style.Add("filter", String.Format("progid:DXImageTransform.Microsoft.AlphaImageLoader(src='{0}', sizingMethod='scale')", _sealPath));
                            
                        divImg.Visible = true;
                    }
                }
            }
        }

    }
}