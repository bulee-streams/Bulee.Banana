using System.Net;
using System.Threading.Tasks;

namespace API.EmailSender
{
    public interface IEmail
    {
        Task<HttpStatusCode> Send(string fromName, string toName, string toAddress, string fromAddress, string subject, string content);
    }
}
