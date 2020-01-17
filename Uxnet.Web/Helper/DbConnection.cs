using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uxnet.Web.Helper
{
    public class DbConnection
    {
        /// <summary>
        /// DBServer LocalDb.
        /// </summary>
        public class LocalDb
        {
            private const string LocalDbConnectionString =
@"Data Source=localhost\sqlexpress;Initial Catalog={0};Integrated Security=True;MultipleActiveResultSets=True";

            public static string InvoiceClient
            {
                get
                {
                    return string.Format(LocalDbConnectionString, "InvoiceClient");
                }
            }
        }
    }
}