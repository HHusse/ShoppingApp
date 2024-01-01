using System;
namespace ShoppingApp.Common.Models
{
    public class InvoiceDetails
    {
        public InvoiceDetails(string email, string invoiceId, string to, string from, string address, string phoneNumber, List<Product> products)
        {
            Email = email;
            InvoiceId = invoiceId;
            To = to;
            From = from;
            Products = products;
            Address = address;
            PhoneNumber = phoneNumber;
        }

        public string Email { get; }
        public string InvoiceId { get; }
        public string To { get; }
        public string From { get; }
        public string Address { get; }
        public string PhoneNumber { get; }
        public List<Product> Products { get; }
    }
}

