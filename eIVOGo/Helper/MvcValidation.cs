using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Model.Models.ViewModel;
using Utility;
using MvcValidationResource = eIVOGo.Resource.Helpers.MvcValidation;

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
                Controller.ModelState.AddModelError("CompanyName", MvcValidationResource.請輸入公司名稱__);
                
            }
            viewModel.ReceiptNo = viewModel.ReceiptNo.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.ReceiptNo))
            {
                //檢查名稱
                Controller.ModelState.AddModelError("ReceiptNo", MvcValidationResource.請輸入公司統編__);
                
            }
            viewModel.Addr = viewModel.Addr.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.Addr))
            {
                //檢查名稱
                Controller.ModelState.AddModelError("Addr", MvcValidationResource.請輸入公司地址__);
                
            }
            viewModel.Phone = viewModel.Phone.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.Phone))
            {
                //檢查名稱
                Controller.ModelState.AddModelError("Phone", MvcValidationResource.請輸入公司電話__);
                
            }

            Regex reg = new Regex("\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*");
            viewModel.ContactEmail = viewModel.ContactEmail.GetEfficientString();
            if (String.IsNullOrEmpty(viewModel.ContactEmail) || !reg.IsMatch(viewModel.ContactEmail))
            {
                //檢查email
                Controller.ModelState.AddModelError("ContactEmail", MvcValidationResource.電子信箱尚未輸入或輸入錯誤__);
            }
        }

    }
}