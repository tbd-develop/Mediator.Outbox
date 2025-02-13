﻿namespace TbdDevelop.Mediator.Outbox.Infrastructure;

public interface IQueueProcessor
{
    Task<QueueStatus> ProcessNextOutboxQueueMessage(CancellationToken cancellationToken);
}