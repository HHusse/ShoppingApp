using System;
using System.Reflection.PortableExecutable;
using System.Text;
using Newtonsoft.Json;
using ShoppingApp.Common.Dependencies;
using ShoppingApp.Common.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShoppingApp.Common.Utils
{
    public class InvoiceMaker : IInvoiceMaker
    {
        public InvoiceDetails InvoiceDetails { get; set; }

        public InvoiceMaker(InvoiceDetails invoiceDetails)
        {
            InvoiceDetails = invoiceDetails;
        }

        public async Task<string> ProcessInvoice()
        {
            using (var httpClient = new HttpClient())
            {
                var requestUri = "https://invoice-generator.com";
                var invoiceRequest = new
                {
                    header = "Factura",
                    to_title = "Client",
                    ship_to_title = "Adresa",
                    date_title = "Data",
                    item_header = "Produs",
                    quantity_header = "Cantitate",
                    unit_cost_header = "Pret/Buc",
                    amount_header = "Pret",
                    balance_title = "Sold de plata",
                    amount_paid_title = "Suma platita",


                    from = "Furnizor:\n" + InvoiceDetails.From,
                    to = InvoiceDetails.To + "\nNumar de telefon: " + InvoiceDetails.PhoneNumber,
                    number = InvoiceDetails.InvoiceId,
                    currency = "RON",
                    ship_to = InvoiceDetails.Address,
                    items = InvoiceDetails.Products.Select(p => new { name = p.Name, quantity = p.Quantity, unit_cost = p.Price }).ToList(),
                    amount_paid = InvoiceDetails.Products.Sum(p => p.Price * p.Quantity),
                    notes = "Mulțumim că ați ales serviciile noastre"
                };

                var jsonContent = JsonConvert.SerializeObject(invoiceRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStreamAsync();
                    var pdfFilePath = await SavePdfToFile(responseData);
                    return pdfFilePath;
                }
                return "";
            }
        }

        private async Task<string> SavePdfToFile(Stream pdfStream)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Invoice.pdf");
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await pdfStream.CopyToAsync(fileStream);
            }
            return filePath;
        }
    }
}

