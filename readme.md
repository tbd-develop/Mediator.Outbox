## Mediator Outbox 

The outbox pattern is a useful pattern for resilience in your messaging. This implementation based on the Mediator
library is intended to place INotification messages produced in the application into a queue for later processing.

### Usage

In your code, where you would normally use IPublisher, or IMediator with Publish, you would instead replace this with 
INotificationPublisher. This is a new interface is functionally compatible with IPublisher interface for Publish.

#### In Memory Outbox, no processing

This is pretty much a useless implementation, as nothing will be done with messages and they exist only in your application.

```csharp
ServiceCollection.AddMediatorOutbox(configure => 
    configure.UseInMemoryOutbox()
    );
```

#### In Memory Outbox, with processing

This is a more useful implementation, as it will process messages in the background. This is useful for testing, but you're basically 
getting the same behavior as the default Mediator implementation.

```csharp
ServiceCollection.AddMediatorOutbox(configure => 
    configure
        .UseInMemoryOutbox()
        .WithBackgroundProcessing()
    );
```

#### SqlServer Outbox with/without processing

This is a more useful implementation, it will persist messages to a SqlServer database. And you can optionally enable processing in the background. This could also provide you the ability 
to only process on a single server of your application in a cluster if you enable processing on one server.

```csharp 
ServiceCollection.AddMediatorOutbox(configure => 
    configure
        .UseSqlServerOutbox("connection string")
        .WithBackgroundProcessing()
    );
```

#### Issues 

* The SqlServer implementation is not yet complete. It is not yet possible to configure the table name
* There is no user tracking, so the user responsible for the events is lost if it's not part of your event.