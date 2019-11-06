using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.Common
{
    public partial class ChooseMultiple : System.Web.UI.UserControl
    {
        public event EventHandler Confirm;
        public event EventHandler Cancel;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public ListBox ApplicableItems
        {
            get
            {
                return applicableList;
            }
        }

        public ListBox SelectedItems
        {
            get
            {
                return selectedList;
            }
        }

        public Button ChooseButton
        {
            get
            {
                return btnSelect;
            }
        }

        public Button RemoveButton
        {
            get
            {
                return btnRemove;
            }
        }

        protected virtual void btnConfirm_Click(object sender, EventArgs e)
        {
            OnConfirm(sender,e);
        }

        protected virtual void btnCancel_Click(object sender, EventArgs e)
        {
            OnCancel(sender, e);
        }

        protected virtual void OnConfirm(object sender, EventArgs e)
        {
            if (Confirm != null)
            {
                Confirm(sender, e);
            }
        }

        protected virtual void OnCancel(object sender, EventArgs e)
        {
            if (Cancel != null)
            {
                Cancel(sender, e);
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            arrangeChoice(applicableList,selectedList);
        }

        private void arrangeChoice(ListBox from,ListBox to)
        {
            ListItem[] items = from.Items.Cast<ListItem>().Where(i => i.Selected).ToArray();
            if (items != null && items.Length > 0)
            {
                to.Items.AddRange(items);
                foreach (var item in items)
                {
                    from.Items.Remove(item);
                }
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            arrangeChoice(selectedList, applicableList);
        }
    }
}