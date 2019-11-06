namespace Uxnet.Web.Module.Common
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Web.UI;
	using System.Collections.Specialized;
using System.ComponentModel;
    using System.Text;
	

	/// <summary>
	///		GeneralDialog 的摘要描述。
	/// </summary>
	public class ConfirmationDialog : System.Web.UI.UserControl , IPostBackDataHandler
	{
		public event EventHandler DoYes;
		public event EventHandler DoNo;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在這裡放置使用者程式碼以初始化網頁
			this.Page.RegisterRequiresPostBack(this);
		}

		public virtual bool LoadPostData(string postDataKey,	NameValueCollection postCollection)
		{
			return true;
		}

		protected void OnYes(EventArgs e)
		{
			if(DoYes!=null)
			{
				DoYes(this,e);
			}
		}

		protected void OnNo(EventArgs e)
		{
			if(DoNo!=null)
			{
				DoNo(this,e);
			}
		}

        [Bindable(true)]
		public string Confirmation
		{
			get
			{
				return (string)this.ViewState["confirm"];
			}
			set
			{
				this.ViewState["confirm"] = value;
			}
		}

        [Bindable(true)]
		public string CommandArgument
		{
			get
			{
				return (string)this.ViewState["arg"];
			}
			set
			{
				this.ViewState["arg"] = value;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			base.Render (writer);
            if (!String.IsNullOrEmpty(Confirmation))
            {
                StringBuilder script = new StringBuilder();
                script.Append("if(confirm('")
                    .Append(Confirmation.Replace("'", "\\'")).Append("')) {")
                    .Append(this.Page.ClientScript.GetPostBackEventReference(this, "yes"))
                    .Append("} else {")
                    .Append(this.Page.ClientScript.GetPostBackEventReference(this, "no"))
                    .Append("}");

                if (ScriptManager.GetCurrent(this.Page) != null)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "confirm", script.ToString(), true);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "confirm", script.ToString(), true);
                }

            }
		}


		public virtual void RaisePostDataChangedEvent()
		{
			if("yes".Equals(Request["__EVENTARGUMENT"]))
			{
				OnYes(new EventArgs());
			}
			else if("no".Equals(Request["__EVENTARGUMENT"]))
			{
				OnNo(new EventArgs());
			}
			this.Visible = false;
		}

	}
}
