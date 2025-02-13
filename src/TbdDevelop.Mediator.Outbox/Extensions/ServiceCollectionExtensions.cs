﻿using Microsoft.Extensions.DependencyInjection;
using TbdDevelop.Mediator.Outbox.Extensions.Configuration;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.Outbox;

namespace TbdDevelop.Mediator.Outbox.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatorOutbox(this IServiceCollection services,
        Action<MediatorOutboxConfigurationBuilder> configure)
    {
        var builder = new MediatorOutboxConfigurationBuilder(services);

        configure(builder);

        if (services.All(s => s.ServiceType != typeof(IOutboxProcessingPublisher)))
        {
            services.AddSingleton<IOutboxProcessingPublisher, DefaultOutboxProcessingPublisher>();
        }

        return services;
    }
}