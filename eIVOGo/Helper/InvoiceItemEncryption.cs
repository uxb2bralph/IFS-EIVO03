using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Model.DataEntity;
using Utility;

namespace eIVOGo.Helper
{
    public class InvoiceItemEncryption
    {
        private String _key;
        public InvoiceItemEncryption()
        {
            String keyFile = Path.Combine(Logger.LogPath, "ORCodeKey.txt");
            if (!File.Exists(keyFile))
                return;

            _key = File.ReadAllText(keyFile);
        }

        public String EncryptContent(InvoiceItem item)
        {
            if(!String.IsNullOrEmpty(_key) && item!=null)
            {
                String content = item.TrackCode + item.No + item.RandomNo;
                return encryptContent(content);
            }
            return null;
        }

        public String EncryptContent(String invoiceNo,String randomNo)
        {
            return encryptContent(invoiceNo + randomNo);
        }


        private string encryptContent(string content)
        {
            com.tradevan.qrutil.QREncrypter qrencrypter = new com.tradevan.qrutil.QREncrypter();
            return qrencrypter.AESEncrypt(content, _key);
        }
    }
}