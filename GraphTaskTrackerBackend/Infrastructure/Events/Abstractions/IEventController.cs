using Microsoft.AspNetCore.Mvc;

namespace GraphTaskTrackerBackend.Infrastructure.Events.Abstractions;

public interface IEventController<TKey, TEvent>
{
    public Task PostAsync(TKey key,TEvent dto);
    public IAsyncEnumerable<TEvent> SubscribeAsync(TKey key, CancellationToken token);
}