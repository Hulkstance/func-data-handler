namespace DataHandler.Core.Events;

public sealed class DataReceivedEventArgs : EventArgs
{
    public DataReceivedEventArgs(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
