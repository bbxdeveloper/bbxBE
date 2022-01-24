using bbxBE.Application.DTOs.Email;
using System.Threading.Tasks;

namespace bbxBE.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}