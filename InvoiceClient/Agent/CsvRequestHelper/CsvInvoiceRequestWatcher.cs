using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

using InvoiceClient.Helper;
using InvoiceClient.Properties;
using Model.Locale;
using Model.Schema.EIVO;
using Model.Schema.EIVO.B2B;
using Model.Schema.TXN;
using Utility;
using CsvHelper;
using InvoiceClient.WS_Invoice;
using System.Xml.Linq;
using XmlLib;

namespace InvoiceClient.Agent.CsvRequestHelper
{
    public class CsvInvoiceRequestWatcher : CsvRequestWatcher
    {
        XElement _invoice, _root;
        List<String[]> _master, _details;
        public CsvInvoiceRequestWatcher(String fullPath)
            : base(fullPath)
        {

        }

        protected override void processFile(string invFile)
        {
            String fileName = Path.GetFileName(invFile);
            String originalPath = Path.GetDirectoryName(invFile);

            String master = fileName.Contains("master")
                    ? fileName
                    : fileName.Contains("detail")
                        ? fileName.Replace("detail", "master")
                        : fileName;
            String detail = master.Replace("master", "detail");

            base.processFile(Path.Combine(originalPath,master));
            _master = DataItems;

            base.processFile(Path.Combine(originalPath, detail));
            _details = DataItems;

            _root = _invoice = null;
            if (_master != null && _master.Any() && _details!=null && _details.Any())
            {
                foreach(var col in _master)
                {
                    col[0] = col[0].GetEfficientString();
                }

                foreach (var col in _details)
                {
                    col[0] = col[0].GetEfficientString();
                }

                processUpload();
                _root.Save(Path.Combine(_ResponsedPath, $"{Path.GetFileNameWithoutExtension(fileName)}.xml"));
            }
        }

        protected virtual void processUpload()
        {
            _root = new XElement("InvoiceRoot");
            foreach (var invRow in DataItems)
            {
                _invoice = new XElement("Invoice",
                    new XElement("InvoiceNumber", invRow[0]),
                    new XElement("InvoiceDate", invRow[1]),
                    new XElement("InvoiceTime", invRow[2]),
                    new XElement("SellerId", invRow[3]),
                    new XElement("BuyerName", invRow[13]),
                    new XElement("BuyerId", invRow[12]),
                    new XElement("BuyerMark", invRow[22]),
                    new XElement("CustomsClearanceMark", invRow[24]),
                    new XElement("DonateMark", invRow[29]),
                    new XElement("SalesAmount", invRow[31]),
                    new XElement("TaxType", invRow[32]),
                    new XElement("TaxRate", invRow[33]),
                    new XElement("TaxAmount", invRow[34]),
                    new XElement("TotalAmount", invRow[35]),
                    new XElement("Currency", invRow[39]),
                    new XElement("DiscountAmount", invRow[36]),
                    new XElement("CustomerID", invRow[19]),
                    new XElement("ContactName", invRow[15]),
                    new XElement("EMail", invRow[18]),
                    new XElement("Address", invRow[14]),
                    new XElement("Phone", invRow[16]),
                    new XElement("MainRemark", invRow[23]),
                    new XElement(new XElement("Contact",
                        new XElement("Name", invRow[15]),
                        new XElement("Address", invRow[14]),
                        new XElement("TEL", invRow[16]),
                        new XElement("Email", invRow[18]))),
                    new XElement("BondedAreaConfirm", invRow[30])
                );

                foreach(var col in _details.Where(d => d[0] == invRow[0]))
                {
                    _invoice.Add(new XElement("InvoiceItem",
                        new XElement("Description", col[1]),
                        new XElement("Quantity", col[2]),
                        new XElement("Unit", col[3]),
                        new XElement("UnitPrice", col[4]),
                        new XElement("Amount", col[5]),
                        new XElement("SequenceNumber", col[1]),
                        new XElement("Item", col[7]),
                        new XElement("Remark", col[6])
                    ));
                }

                _root.Add(_invoice);
            }
        }


    }
}
