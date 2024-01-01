using System;
using ShoppingApp.Common.Models;

namespace ShoppingApp.Common.Dependencies
{
    public interface IInvoiceMaker
    {
        public InvoiceDetails InvoiceDetails { get; set; }
        public Task<string> ProcessInvoice();
    }
}

