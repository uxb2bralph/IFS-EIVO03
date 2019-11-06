using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utility;
using System.IO;

namespace Uxnet.Web.Module.Common
{
    public partial class ImageUploader : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnPreview.OnClientClick = String.Format("view(document.all('{0}'),document.all('{1}'));return false;", newSign.ClientID, imgFile.ClientID);
            Page.ClientScript.RegisterClientScriptBlock(typeof(ImageUploader), "view", @"
                        function view(anchor,uploadField) {
                            if (uploadField.value.length == 0) {
                              alert (""您沒有選擇要上傳的檔案!!"");
                            }
                            else
                            {
                                anchor.style.visibility = 'visible';
                                anchor.innerHTML = ""<img width='50' height='50' src='"" + uploadField.value + ""'/>"";
                            }
                        }
                    ", true);

            if (this.IsPostBack && imgFile.HasFile)
            {
                saveFile();
            }
        }

        private void saveFile()
        {
            String storePath = String.IsNullOrEmpty(ImageBaseUrl) ? "" : this.MapPath(ImageBaseUrl);
            ImageFileName = ValueValidity.SaveUploadFile(imgFile.PostedFile, storePath, Path.GetExtension(imgFile.PostedFile.FileName));          
        }

        public String ImageBaseUrl
        {
            get
            {
                return ViewState["baseUrl"] as String;
            }
            set
            {
                ViewState["baseUrl"] = value;
            }
        }

        public String ImageFileName
        {
            get
            {
                return ViewState["imgName"] as String;
            }
            set
            {
                ViewState["imgName"] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(ImageUploader_PreRender);
        }

        void ImageUploader_PreRender(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(ImageFileName))
            {
                if (!String.IsNullOrEmpty(ImageBaseUrl))
                    this.imgInvoice.ImageUrl = VirtualPathUtility.Combine(ImageBaseUrl, ImageFileName);
                else
                    this.imgInvoice.ImageUrl = ImageFileName;
            }
        }
    }
}