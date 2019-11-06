using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eIVOGo.Module.EIVO.Item;
using Model.InvoiceManagement;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Business.Helper;
using Model.Locale;

namespace eIVOGo.SAM
{
    public partial class PrintReceiptPage : System.Web.UI.Page
    {
        private UserProfileMember _userProfile;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
            initializeData();
        }

        protected virtual void initializeData()
        {
            //IEnumerable<int> items = Session["PrintDoc"] as IEnumerable<int>;
            //Session.Remove("PrintDoc");


            //IEnumerable<int> unPrintID = Session["UnPrintDoc"] as IEnumerable<int>;
            //Session.Remove("UnPrintDoc"); //用來判斷此份收據是否為正本(未列印)
            IEnumerable<int> items;
            using (InvoiceManager mgr = new InvoiceManager())
            {
                items = mgr.GetTable<DocumentPrintQueue>().Where(i => i.UID == _userProfile.UID & i.CDS_Document.DocType == (int)Naming.DocumentTypeDefinition.E_Receipt).Select(i => i.DocID).ToList();
            }


            if ( items!=null )
            {
                SOGOReceiptView finalView = null;
                var mgr = dsEntity.CreateDataManager();

                foreach (var item in items)
                {
                    SOGOReceiptView view = (SOGOReceiptView)this.LoadControl("~/Module/EIVO/Item/SOGOReceiptView.ascx");
                    finalView = view;
                    view.InitializeAsUserControl(this.Page);
                    view.Item = mgr.EntityList.Where(r => r.ReceiptID == item).FirstOrDefault();
                    //if (unPrintID.Contains(item))
                    //    view.IsUnPrint = true; //收據是正本

                    theForm.Controls.Add(view);
                }

                if (finalView != null)
                    finalView.IsFinal = true;
            }
        }
    }
}