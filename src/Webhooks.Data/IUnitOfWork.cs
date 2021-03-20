using System.Threading.Tasks;

namespace Webhooks.Data
{
    public interface IUnitOfWork
    {
        Task<int> Save();
    }
}
