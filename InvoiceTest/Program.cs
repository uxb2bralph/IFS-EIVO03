using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Utility;

namespace InvoiceTest
{
    class Program
    {
        static void Main(string[] args)
        {
//            testJoin();
            Naming.DocumentTypeDefinition? a = null;

            testDeserialize();
            testSchema();
            Console.ReadKey();
        }

        private static void testDeserialize()
        {
            var doc = new XmlDocument();
            doc.Load("Org.xml");
            var items = doc.DeserializeDataContract<Organization[]>();
        }

        private static void testSchema()
        {
            Organization[] items;
            using (InvoiceManager mgr = new InvoiceManager())
            {
                items = mgr.GetTable<Organization>().ToArray();
                if (items.Length > 0)
                {
                    var item = items[0];
                    //var values = item.GetType().GetProperties().Select(p => p.GetValue(item, null)).ToArray();
                    foreach (var p in item.GetType().GetProperties())
                    {
                        //Convert.ChangeType(p.GetValue(item, null), p.PropertyType);
                        var obj = p.GetValue(item, null);
                    }
                }
            }

        }

        class _key
        {
            public int? a;
            public int? b;
        };

        private static void testJoin()
        {

            using (InvoiceManager mgr = new InvoiceManager())
            {

                //var items = mgr.EntityList
                //    .Join(mgr.GetTable<BusinessRelationship>() , i => new _key { a = i.InvoiceSeller.SellerID, b = i.InvoiceBuyer.BuyerID }, r => new _key { a = r.MasterID, b = r.RelativeID }, (i, r) => r);
                //Console.WriteLine(items.Count());

                var dispatch = mgr.GetTable<DocumentDispatch>();
                var items = dispatch.ToList();

                var toIssue = mgr.GetTable<CDS_Document>().Where(d => d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待開立);

                var notifyToIssue = toIssue
                        .Join(mgr.EntityList, t => t.DocID, i => i.InvoiceID, (t, i) => i)
                        .Join(dispatch, t => t.InvoiceID, s => s.DocID, (t, s) => new _key { a = t.InvoiceSeller.SellerID, b = t.InvoiceBuyer.BuyerID })
                        .Join(mgr.GetTable<BusinessRelationship>(), k => k, r => new _key { a = r.RelativeID, b = r.MasterID }, (k, r) => k);
                    //    .Select(d => d.InvoiceItem.SellerID)
                    //.Concat(toIssue
                    //    .Join(mgr.GetTable<DerivedDocument>()
                    //        .Join(dispatch.Where(d => d.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.作廢發票)
                    //            , r => r.SourceID, s => s.DocID, (r, s) => r)
                    //        , d => d.DocID, r => r.DocID, (d, r) => r)
                    //    .Select(r => r.ParentDocument.InvoiceItem.SellerID))
                    //.Concat(toIssue
                    //    .Join(dispatch.Where(d => d.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.發票折讓), t => t.DocID, s => s.DocID, (t, s) => t)
                    //    .Select(d => d.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID))
                    //.Concat(toIssue
                    //    .Join(mgr.GetTable<DerivedDocument>()
                    //        .Join(dispatch.Where(d => d.TypeID == (int)Naming.B2BInvoiceDocumentTypeDefinition.作廢折讓)
                    //            , r => r.SourceID, s => s.DocID, (r, s) => r)
                    //        , d => d.DocID, r => r.DocID, (d, r) => r)
                    //    .Select(r => r.ParentDocument.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID))
                    //.Join(mgr.GetTable<BusinessRelationship>(), b => b, r => r.RelativeID, (b, r) => b)
                    //.Distinct();

                Console.WriteLine(notifyToIssue.Count());

            }
        }
    }
}
