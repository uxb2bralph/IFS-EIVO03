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
    public partial class RealSizeImage : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void BindData(string imgUrl)
        {
            realImg.Src = imgUrl;
            this.DataBind();
        }

        public string ImageUrl
        {
            get
            {
                return realImg.Src;
            }
            set
            {
                realImg.Src = value;
            }
        }


        public override void DataBind()
        {
            if (!String.IsNullOrEmpty(realImg.Src))
            {
                base.DataBind();

                string sealPath = Server.MapPath(realImg.Src);

                if (File.Exists(sealPath))
                {
                    using (Bitmap bmp = new Bitmap(sealPath))
                    {
                        realImg.Style.Add("height", String.Format("{0}in", (double)bmp.Height / (double)bmp.HorizontalResolution));
                        realImg.Style.Add("width", String.Format("{0}in", (double)bmp.Width / (double)bmp.VerticalResolution));
                        realImg.Visible = true;
                    }
                }
            }
        }

    }
}