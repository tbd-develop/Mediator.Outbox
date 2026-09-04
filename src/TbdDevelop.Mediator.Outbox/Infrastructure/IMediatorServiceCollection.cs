using Microsoft.Extensions.DependencyInjection;

namespace TbdDevelop.Mediator.Outbox.Infrastructure;

public interface IMediatorServiceCollection : IServiceCollection
{
    void AddInServiceLifetime<TImplementation>()
        where TImplementation : class;

    void AddInServiceLifetime<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;
}