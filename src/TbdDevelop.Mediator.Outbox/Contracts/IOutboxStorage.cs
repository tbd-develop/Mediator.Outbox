﻿namespace TbdDevelop.Mediator.Outbox.Contracts;

public interface IOutboxStorage
{
    public Task<IOutboxMessage?> RetrieveNextMessage(CancellationToken cancellationToken = default);

    public Task Commit(IOutboxMessage message, CancellationToken cancellationToken = default);
}