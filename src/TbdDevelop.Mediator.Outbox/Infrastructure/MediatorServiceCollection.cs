using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace TbdDevelop.Mediator.Outbox.Infrastructure;

public sealed class MediatorServiceCollection(
    ServiceLifetime serviceLifetime,
    IServiceCollection collection) : IMediatorServiceCollection
{
    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(
        ServiceDescriptor item
    )
    {
        collection.Add(item);
    }

    public void Clear()
    {
        collection.Clear();
    }

    public bool Contains(
        ServiceDescriptor item
    )
    {
        return collection.Contains(item);
    }

    public void CopyTo(
        ServiceDescriptor[] array,
        int arrayIndex
    )
    {
        collection.CopyTo(array, arrayIndex);
    }

    public bool Remove(
        ServiceDescriptor item
    )
    {
        return collection.Remove(item);
    }

    public int Count => collection.Count;
    public bool IsReadOnly => collection.IsReadOnly;

    public int IndexOf(
        ServiceDescriptor item
    )
    {
        return collection.IndexOf(item);
    }

    public void Insert(
        int index,
        ServiceDescriptor item
    )
    {
        collection.Insert(index, item);
    }

    public void RemoveAt(
        int index
    )
    {
        collection.RemoveAt(index);
    }

    public ServiceDescriptor this[
        int index
    ]
    {
        get => collection[index];
        set => collection[index] = value;
    }

    public void AddInServiceLifetime<TImplementation>() where TImplementation : class
    {
        if ( serviceLifetime == ServiceLifetime.Scoped )
        {
            collection.AddScoped<TImplementation>();
        }
        else
        {
            collection.AddTransient<TImplementation>();
        }
    }

    public void AddInServiceLifetime<TService, TImplementation>()
        where TService : class where TImplementation : class, TService
    {
        if ( serviceLifetime == ServiceLifetime.Scoped )
        {
            collection.AddScoped<TService, TImplementation>();
        }
        else
        {
            collection.AddTransient<TService, TImplementation>();
        }
    }
}