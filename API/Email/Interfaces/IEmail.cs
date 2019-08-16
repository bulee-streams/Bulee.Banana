using System.Net;
using System.Threading.Tasks;

namespace API.Email.Interfaces
{
    public interface IEmail
    {
        Task<HttpStatusCode> Send(string fromName, string toName, string toAddress, string fromAddress, string templateId, object data);
    }
}
