using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Contracts;
using TbdDevelop.Mediator.Outbox.Extensions.Configuration;
using TbdDevelop.Mediator.Outbox.Outbox;

namespace TbdDevelop.Mediator.Outbox.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorOutbox(this IServiceCollection services,
        Action<MediatorOutboxConfigurationBuilder> configure)
    {
        var builder = new MediatorOutboxConfigurationBuilder(services);

        configure(builder);

        return services;
    }
}