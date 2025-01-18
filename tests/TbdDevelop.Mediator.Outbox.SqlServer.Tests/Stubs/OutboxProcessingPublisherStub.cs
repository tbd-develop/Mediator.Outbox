﻿using Org.BouncyCastle.Crypto;
using TbdDevelop.Mediator.Outbox.Infrastructure;
using TbdDevelop.Mediator.Outbox.SqlServer.Tests.Models;

namespace TbdDevelop.Mediator.Outbox.SqlServer.Tests.Stubs;

public class OutboxProcessingPublisherStub : IOutboxProcessingPublisher
{
    public ValueTask Publish<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        switch (@event)
        {
            case SampleEventType sample:
                Console.WriteLine("Publishing event: {content}" + sample.Content);
                break;
            case SampleErrorEventType error:
                Console.WriteLine("Publishing error event: " + error.Content);

                throw new MaxBytesExceededException("Too many bytes");
        }

        return ValueTask.CompletedTask;
    }
}