using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;

namespace eIVOGo.Module.SYS.Item
{
    public partial class AnnouncementItem : System.Web.UI.UserControl
    {
        public event EventHandler Done;
        private UserProfileMember _userProfile;
        private Announcement_REC _dataItem;
        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.dsEntity.Select += new EventHandler<DataAccessLayer.basis.LinqToSqlDataSourceEventArgs<Announcement_REC>>(dsEntity_Select);
            this.PreRender += new EventHandler(AnnouncementItem_PreRender);

        }
        void AnnouncementItem_PreRender(object sender, EventArgs e)
        {
            if (_dataItem != null)
            {
                if (_dataItem.StartDate.HasValue)
                    DateFrom.DateTimeValue = _dataItem.StartDate.Value;
                else
                    DateFrom.Reset();
                if (_dataItem.EndDate.HasValue)
                EndDate.DateTimeValue = _dataItem.EndDate.Value;
                else
                    EndDate.Reset();
            }
            else
            {
                this.txtAnnID.Text = string.Empty;
                DateFrom.Reset();
                EndDate.Reset();
            }
        }
        public void Clean()
        {
            DataItem = null;
            this.txtAnnMessage.Text = "";
            DateFrom.Reset();
            EndDate.Reset();
            this.AlwaysShow.Checked = true;
        }
        void dsEntity_Select(object sender, DataAccessLayer.basis.LinqToSqlDataSourceEventArgs<Announcement_REC> e)
        {
            if (QueryExpr != null)
            {
                e.QueryExpr = QueryExpr;
            }
            else
            {
                e.QueryExpr = r => false;
            }
        }

        public Expression<Func<Announcement_REC, bool>> QueryExpr
        { get; set; }

        public void Show()
        {
            //dvEntity.ChangeMode(DetailsViewMode.Insert);
            this.ModalPopupExtender.Show();
        }

        public void BindData()
        {
            //dvEntity.ChangeMode(DetailsViewMode.Edit);
            //dvEntity.DataBind();
            _dataItem = dsEntity.CreateDataManager().EntityList.Where(QueryExpr).FirstOrDefault();

            base.DataBind();
            this.ModalPopupExtender.Show();

        }

        protected void dvEntity_ItemCommand(object sender, DetailsViewCommandEventArgs e)
        {
            if (e.CommandName == "Cancel")
            {
                this.ModalPopupExtender.Hide();
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.ModalPopupExtender.Hide();
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (checkdata())
            {
                if (string.IsNullOrEmpty(txtAnnID.Text))
                    dsEntity_ItemInserting();
                else
                    dsEntity_ItemUpdating();
                this.ModalPopupExtender.Hide();
            }
        }
        protected void dsEntity_ItemInserting()
        {

                try
                {
                    var mgr = dsEntity.CreateDataManager();
                    Announcement_REC item = new Announcement_REC
                    {

                        StartDate = string.IsNullOrEmpty(this.DateFrom.TextBox.Text) ?
                        (DateTime?)null : Convert.ToDateTime(this.DateFrom.TextBox.Text),
                        EndDate = string.IsNullOrEmpty(this.EndDate.TextBox.Text) ?
                         (DateTime?)null : Convert.ToDateTime(this.EndDate.TextBox.Text),
                        //DateTime.ParseExact(String.Format("{0} {1}", this.EndDate.TextBox.Text), "yyyy/MM/dd", System.Globalization.CultureInfo.CurrentCulture),
                        AnnMessage = txtAnnMessage.Text,
                        Creator = _userProfile.UID,
                        CreateTime = DateTime.Now,
                        AlwaysShow = AlwaysShow.Checked
                    };
                    mgr.EntityList.InsertOnSubmit(item);
                    mgr.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    this.AjaxAlert(String.Format("新增資料失敗,原因:{0}", ex.Message));
                }
                Response.Redirect(Request.RawUrl);
        }
        public Announcement_REC DataItem
        {
            get
            {
                return _dataItem;
            }
            set
            {
                _dataItem = value;

            }
        }

        protected void dvEntity_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (Done != null)
            {
                Done(this, new EventArgs());
            }
        }

        protected void dvEntity_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            if (Done != null)
            {
                Done(this, new EventArgs());
            }
        }
        protected void dsEntity_ItemUpdating()
        {

                try
                {
                    var mgr = dsEntity.CreateDataManager();
                    var item = mgr.EntityList.Where(r => r.AnnID == Convert.ToInt16(txtAnnID.Text)).FirstOrDefault();
                    item.AnnMessage = txtAnnMessage.Text;
                    if (!string.IsNullOrEmpty(this.DateFrom.TextBox.Text))
                        item.StartDate = Convert.ToDateTime(this.DateFrom.TextBox.Text);
                    if (!string.IsNullOrEmpty(this.EndDate.TextBox.Text))
                        item.EndDate = Convert.ToDateTime(this.EndDate.TextBox.Text);
                    //item.Creator = _userProfile.UID;
                    //item.CreateTime = DateTime.Now;
                    item.AlwaysShow = AlwaysShow.Checked;

                    mgr.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    this.AjaxAlert(String.Format("修改資料失敗,原因:{0}", ex.Message));
                }

                Response.Redirect(Request.RawUrl);
        }
        protected void dvEntity_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {

                try
                {
                    var mgr = dsEntity.CreateDataManager();
                    var item = mgr.EntityList.Where(r => r.AnnID == (int)e.Keys[0]).First();
                    item.AnnMessage = (String)e.NewValues["AnnMessage"];
                    item.StartDate = (DateTime)e.NewValues["DateFrom"];
                    item.EndDate = (DateTime)e.NewValues["DateTo"];
                    item.Creator = _userProfile.UID;
                    item.CreateTime = DateTime.Now;
                    item.AlwaysShow = (bool)e.NewValues["AlwaysShow"];
                    mgr.SubmitChanges();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    this.AjaxAlert(String.Format("修改資料失敗,原因:{0}", ex.Message));
                }
            
        }

        private bool checkdata()
        {
            if (String.IsNullOrEmpty(this.txtAnnMessage.Text.Trim()))
            {
                this.AjaxAlert("未填寫訊息內容!!");
                return false;
            }
            if(string.IsNullOrEmpty(this.EndDate.TextBox.Text)&&!this.AlwaysShow.Checked)
            {
                this.AjaxAlert("請確認訊息結束時間或是否永久顯示");
                return false;
            }
            return true;
        }
    }
}