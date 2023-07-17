using SvcEmail.Models;

namespace SvcEmail.Interface
{
    public interface IEmail
    {
        Task<T> configEmail<T>();
        Task<T> sentEmailAsync<T>(Email Email);

    }
}
