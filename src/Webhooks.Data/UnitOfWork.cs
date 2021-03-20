using Webhooks.Data.Infrastructure;
using System.Threading.Tasks;

namespace Webhooks.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly WebhooksDataContext _dataContext;
        public UnitOfWork(WebhooksDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<int> Save()
        {
           return await _dataContext.SaveChangesAsync();
        }
    }
}
