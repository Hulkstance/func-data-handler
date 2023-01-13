using System.Diagnostics.CodeAnalysis;
using DataHandler.Core.Models;

namespace DataHandler.Core;

public sealed class SubscriptionCollection
{
    private readonly HashSet<Subscription> _subscriptions = new();

    public void Add(Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        lock (_subscriptions)
        {
            _subscriptions.Add(subscription);
        }
    }

    public bool Remove(Subscription subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription);

        lock (_subscriptions)
        {
            return _subscriptions.Remove(subscription);
        }
    }

    public void Reset()
    {
        lock (_subscriptions)
        {
            _subscriptions.Clear();
        }
    }

    public HashSet<Subscription> GetAll()
    {
        lock (_subscriptions)
        {
            return _subscriptions;
        }
    }

    public Subscription? GetById(long id)
    {
        lock (_subscriptions)
        {
            return _subscriptions.SingleOrDefault(s => s.Id == id);
        }
    }

    public bool TryGetValue(long id, [NotNullWhen(true)] out Subscription? subscription)
    {
        lock (_subscriptions)
        {
            subscription = _subscriptions.SingleOrDefault(s => s.Id == id);
            return subscription != null;
        }
    }
}
