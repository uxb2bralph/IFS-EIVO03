    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using System;
namespace Model.DataEntity
{
    public partial class EIVOEntityDataContext
    {
        partial void OnCreated()
        {
            this.CommandTimeout = 86400;
        }
    }
         
}
