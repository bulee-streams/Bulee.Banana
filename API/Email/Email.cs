using System.Net;
using System.Threading.Tasks;
using API.EmailSender;
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

        public async Task<HttpStatusCode> Send(string fromName, string toName, string toAddress, string fromAddress, string subject, string content)
        {
            var to = new EmailAddress(toAddress, toName);
            var from = new EmailAddress(fromAddress, fromName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, null);

            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }
    }
}
