using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.basis;
using Model.DataEntity;
using Model.Locale;

namespace Model.Helper
{
    public static class DataStoreExtensionMethods
    {
        public static void Save(this InvoiceEntity entity)
        {
            using (ModelSource<InvoiceItem> models = new ModelSource<InvoiceItem>())
            {
                var item = models.ConvertToInvoiceItem(entity);
                if (item != null)
                {
                    models.GetTable<InvoiceItem>().InsertOnSubmit(item);
                    models.SubmitChanges();
                    entity.Status = Naming.UploadStatusDefinition.匯入成功;
                }
            }
        }
    }
}
