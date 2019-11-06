using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using Model.DocumentFlowManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Uxnet.Web.Module.DataModel;
using Uxnet.Web.WebUI;

namespace eIVOGo.Module.Flow
{
    public partial class DocumentTypeFlowItem : EditEntityItemModal<FlowEntityDataContext, DocumentTypeFlow>
    {
        private int? _typeID;
        private int? _flowID;
        private int? _companyID;
        private int? _businessID;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (modelItem.DataItem is int[])
            {
                int[] keyValue = (int[])modelItem.DataItem;
                _typeID = keyValue[0];
                _flowID = keyValue[1];
                _companyID = keyValue[2];
            }

            this.QueryExpr = m => m.TypeID == _typeID && m.FlowID == _flowID && m.CompanyID == _companyID && m.BusinessID == _businessID;
        }

        protected override bool saveEntity()
        {
            if (String.IsNullOrEmpty(TypeID.SelectedValue))
            {
                this.AjaxAlert("請選擇文件類型!!");
                return false;
            }

            _typeID = int.Parse(TypeID.SelectedValue);

            if (String.IsNullOrEmpty(CompanyID.SelectedValue))
            {
                this.AjaxAlert("請選擇適用的公司!!");
                return false;
            }

            _companyID = int.Parse(CompanyID.SelectedValue);

            if (String.IsNullOrEmpty(FlowID.SelectedValue))
            {
                this.AjaxAlert("請選擇套用的流程!!");
                return false;
            }

            _flowID = int.Parse(FlowID.SelectedValue);

            if (String.IsNullOrEmpty(BusinessID.SelectedValue))
            {
                this.AjaxAlert("請選擇發票屬性!!");
                return false;
            }

            _businessID = int.Parse(BusinessID.SelectedValue);

            loadEntity();

            var mgr = dsEntity.CreateDataManager();

            if (_entity == null)
            {
                _entity = new DocumentTypeFlow
                {
                    FlowID = _flowID.Value,
                    CompanyID = _companyID.Value,
                    TypeID = _typeID.Value,
                    BusinessID = _businessID.Value
                };

                mgr.EntityList.InsertOnSubmit(_entity);
                mgr.SubmitChanges();
                this.AjaxAlert("流程套用設定完成!!");
                return true;
            }
            else 
            {
                this.AjaxAlert("該公司所套用的流程已存在,不可以重複設定!!");
                return false;
            }
        }
    }
}