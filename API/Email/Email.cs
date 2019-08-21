using System;
using System.Net;
using System.Threading.Tasks;
using API.Email.Interfaces;
using API.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace API.Email
{
 
    public class Email : IEmail
    {
        private readonly string templateId;
        private readonly SendGridClient client;

        public Email(IConfiguration configuration)
        {
            var config = configuration ?? throw new ArgumentNullException(nameof(configuration));

            templateId = config["Keys:ConfirmationEmailId"];
            client = new SendGridClient(config["ConnectionStrings:SendGridAPIKey"]);
        }

        public async Task<HttpStatusCode> Send(string fromName, string toName, string toAddress, string fromAddress, string templateId, object data)
        {
            var to = new EmailAddress(toAddress, toName);
            var from = new EmailAddress(fromAddress, fromName);
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, data);

            var response = await client.SendEmailAsync(msg);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> SendCofirmationEmail(string username, string emailAddress, Guid emailToken)
        {
            var url = BananaHttpContext.AppBaseUrl + "/api/v1/users/registration-complete/" + emailToken;
            var objectData = new ConfirmationEmail() { Username = username, Url = url };

            var result = await Send("Bulee Services", username, emailAddress,
                                    "banana@bulee.com", templateId, objectData);

            return result;
        }
    }
}
