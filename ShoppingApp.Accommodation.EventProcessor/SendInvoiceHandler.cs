using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using ShoppingApp.Common.Models;
using ShoppingApp.Events;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using ShoppingApp.Common.Dependencies;
using ShoppingApp.Common.Utils;

namespace ShoppingApp.Accommodation.EventProcessor
{
    public class SendInvoiceHandler : IEventHandler
    {
        IInvoiceMaker? invoiceMaker;

        public async Task HandleAsync<T>(T tObject) where T : class
        {
            if (tObject is InvoiceDetails invoiceDetails)
            {
                invoiceMaker = new InvoiceMaker(invoiceDetails);
                var pdfFilePath = invoiceMaker.ProcessInvoice().Result;
                SendEmailWithPdf(invoiceDetails.To, invoiceDetails.Email, pdfFilePath, invoiceDetails.InvoiceId).Wait();
                File.Delete(pdfFilePath);
            }
            else
            {
                throw new InvalidOperationException("Handler can only process InvoiceDetails objects.");
            }
        }


        private async Task SendEmailWithPdf(string clientName, string emailAddress, string pdfFilePath, string invoiceId)
        {
            string user = Environment.GetEnvironmentVariable("CREDENTIALUSER")!;
            string password = Environment.GetEnvironmentVariable("CREDENTIALPASS")!;
            using (var smtpClient = new SmtpClient("smtp-mail.outlook.com", 587))
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("shoppingapppssc@outlook.com", "ShoppingExpress"),
                    Subject = $"Factura #{invoiceId}",
                    Body = $"Dragă {clientName},\n\nVrem să-ți exprimăm sinceră noastră recunoștință pentru " +
                    $"că ai ales să cumperi de la ShoppingExpress. Fiecare client este important pentru noi, iar " +
                    $"susținerea ta ne ajută să creștem și să ne îmbunătățim continuu.\n\nSperăm că experiența ta de cumpărături " +
                    $"a fost plăcută și că produsele noastre vor răspunde așteptărilor tale. Dacă ai întrebări sau feedback, te rugăm " +
                    $"să nu eziti să ne contactezi.\n\nMulțumim din nou pentru alegerea făcută și abia așteptăm să te revedem în magazinul " +
                    $"nostru!\nCu multă apreciere,\n\nEchipa ShoppingExpress",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(emailAddress);
                mailMessage.Attachments.Add(new Attachment(pdfFilePath));

                smtpClient.Credentials = new NetworkCredential("shoppingapppssc@outlook.com", "?032lMa@u)Sn2e8RiY");
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}