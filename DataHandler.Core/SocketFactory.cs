using DataHandler.Core.Events;
using DataHandler.Core.Internal;

namespace DataHandler.Core;

public sealed class SocketFactory
{
    private readonly AsyncEvent<DataReceivedEventArgs> _dataReceivedEvent = new();

    public event Func<DataReceivedEventArgs, ValueTask> DataReceivedAsync
    {
        add => _dataReceivedEvent.AddHandler(value);
        remove => _dataReceivedEvent.RemoveHandler(value);
    }

    public async ValueTask EmitAsync(string json)
    {
        await _dataReceivedEvent.InvokeAsync(new DataReceivedEventArgs(json));
    }
}
