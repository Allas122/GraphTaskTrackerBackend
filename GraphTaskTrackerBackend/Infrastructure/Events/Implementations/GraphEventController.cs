using System.Collections.Concurrent;
using System.Threading.Channels;
using GraphTaskTrackerBackend.Infrastructure.Events.Abstractions;
using GraphTaskTrackerBackend.Infrastructure.Events.Implementations.Messages;

namespace GraphTaskTrackerBackend.Infrastructure.Events.Implementations;

public class GraphEventController(ILogger<GraphEventController> logger) : IEventController<Guid, GraphEvent>
{
    private readonly ConcurrentDictionary<Guid,ConcurrentDictionary<Channel<GraphEvent>,byte>> _subscribers = new();
    private readonly ILogger<GraphEventController> _logger = logger;
    
    public async Task PostAsync(Guid graphId, GraphEvent dto)
    {
        if (_subscribers.TryGetValue(graphId, out var channels))
        {
            foreach (var channel in channels.Keys)
            {
                channel.Writer.TryWrite(dto);
            }
        }
        await Task.CompletedTask;
    }

    public async IAsyncEnumerable<GraphEvent> SubscribeAsync(Guid graphId, CancellationToken token)
    {
        var userChannel = Channel.CreateUnbounded<GraphEvent>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true 
        });
        var subscribers = _subscribers.GetOrAdd(graphId, _ => new ConcurrentDictionary<Channel<GraphEvent>, byte>());
        subscribers.TryAdd(userChannel, 0);

        try
        {
            while (await userChannel.Reader.WaitToReadAsync(token))
            {
                while (userChannel.Reader.TryRead(out var ev))
                {
                    yield return ev;
                }
            }
        }
        finally
        {
            if (_subscribers.TryGetValue(graphId, out var subs))
            {
                subs.TryRemove(userChannel, out _);
                if (subs.IsEmpty) _subscribers.TryRemove(graphId, out _);
            }
        }
        
    }
}