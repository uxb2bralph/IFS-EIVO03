using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataEntity;

namespace Model.InvoiceManagement.ErrorHandle
{
    public class MarkToRetryException : Exception
    {
        public MarkToRetryException() : base() { }
        public MarkToRetryException(string message) : base(message) { }
        public MarkToRetryException(string message,Exception innerException) : base(message,innerException) { }
    }

    public class InvoiceNotFoundException : Exception
    {
        public InvoiceNotFoundException() : base() { }
        public InvoiceNotFoundException(string message) : base(message) { }
        public InvoiceNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class DuplicateDataNumberException : Exception
    {
        public DuplicateDataNumberException() : base() { }
        public DuplicateDataNumberException(string message) : base(message) { }
        public DuplicateDataNumberException(string message, Exception innerException) : base(message, innerException) { }
        public InvoicePurchaseOrder CurrentPO { get; set; }
    }

    public class DuplicateAllowanceNumberException : Exception
    {
        public DuplicateAllowanceNumberException() : base() { }
        public DuplicateAllowanceNumberException(string message) : base(message) { }
        public DuplicateAllowanceNumberException(string message, Exception innerException) : base(message, innerException) { }
        public InvoiceAllowance CurrentAllowance { get; set; }
    }

}
