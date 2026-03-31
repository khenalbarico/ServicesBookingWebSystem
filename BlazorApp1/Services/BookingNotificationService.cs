using System.Collections.Concurrent;

namespace BlazorApp1.Services;

public sealed class BookingNotificationService
{
    private readonly ConcurrentDictionary<string, List<Func<string, Task>>> _listeners = new();

    public IDisposable Subscribe(string bookingId, Func<string, Task> handler)
    {
        var handlers = _listeners.GetOrAdd(bookingId, _ => []);
        lock (handlers)
        {
            handlers.Add(handler);
        }

        return new Subscription(() =>
        {
            if (_listeners.TryGetValue(bookingId, out var list))
            {
                lock (list)
                {
                    list.Remove(handler);
                    if (list.Count == 0)
                        _listeners.TryRemove(bookingId, out _);
                }
            }
        });
    }

    public async Task PublishAsync(string bookingId, string status)
    {
        if (!_listeners.TryGetValue(bookingId, out var handlers))
            return;

        List<Func<string, Task>> snapshot;
        lock (handlers)
        {
            snapshot = [.. handlers];
        }

        foreach (var handler in snapshot)
            await handler(status);
    }

    private sealed class Subscription(Action dispose) : IDisposable
    {
        private Action? _dispose = dispose;

        public void Dispose()
        {
            Interlocked.Exchange(ref _dispose, null)?.Invoke();
        }
    }
}