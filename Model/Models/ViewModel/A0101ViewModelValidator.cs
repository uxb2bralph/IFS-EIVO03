using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Resource;
using Model.Schema.EIVO;
using Utility;

namespace Model.Models.ViewModel
{
    public partial class A0101ViewModelValidator<TEntity> : A0401ViewModelValidator<TEntity>
        where TEntity : class, new()
    {
        public A0101ViewModelValidator(ModelSource<TEntity> mgr, Organization owner) : base(mgr, owner)
        {

        }

    }
}
