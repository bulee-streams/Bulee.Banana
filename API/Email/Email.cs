using System.Net;
using System.Threading.Tasks;
using API.Email.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace API.Email
{
 
    public class Email : IEmail
    {
        private readonly SendGridClient client;

        public Email()
        {
            client = new SendGridClient(Connections.Get("ConnectionStrings:SendGridAPIKey").Result);
        }

        public async Task<HttpStatusCode> Send(string fromName, string toName, string toAddress, string fromAddress, string templateId, object data)
        {
            var to = new EmailAddress(toAddress, toName);
            var from = new EmailAddress(fromAddress, fromName);
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, data);

            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }
    }
}
