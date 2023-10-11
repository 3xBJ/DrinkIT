using DrinkIT.Domain.BaseClasses;

namespace DrinkIT.Domain.BaseInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<UnitOfWorkResult> SaveEntitiesAsync(CancellationToken cancellationToken);
    }
}
