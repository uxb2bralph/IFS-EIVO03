using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;

namespace eIVOGo.Helper
{
    public static class MvcValidation
    {
        public static void OrganizationValueCheck(this OrganizationViewModel viewModel, Controller Controller)
        {
            viewModel.CompanyName = viewModel.CompanyName.GetEfficientString();
            if (viewModel.CompanyName==null)
            {
                //檢查名稱
                Controller.ModelState.AddModelError("CompanyName", "請輸入公司名稱!!");
                
            }
            viewModel.ReceiptNo = viewModel.ReceiptNo.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.ReceiptNo))
            {
                //檢查名稱
                Controller.ModelState.AddModelError("ReceiptNo", "請輸入公司統編!!");
                
            }
            viewModel.Addr = viewModel.Addr.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.Addr))
            {
                //檢查名稱
                Controller.ModelState.AddModelError("Addr", "請輸入公司地址!!");
                
            }
            viewModel.Phone = viewModel.Phone.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.Phone))
            {
                //檢查名稱
                Controller.ModelState.AddModelError("Phone", "請輸入公司電話!!");
                
            }

            Regex reg = new Regex("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");
            viewModel.ContactEmail = viewModel.ContactEmail.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.ContactEmail) || !reg.IsMatch(viewModel.ContactEmail))
            {
                //檢查email
                Controller.ModelState.AddModelError("ContactEmail", "電子信箱尚未輸入或輸入錯誤!!");
            }
        }

    }
}