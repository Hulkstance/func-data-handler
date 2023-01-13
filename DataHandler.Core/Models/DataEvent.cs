namespace DataHandler.Core.Models;

public sealed class DataEvent<T>
{
    public DataEvent(T data, DateTimeOffset timestamp)
    {
        Data = data;
        Timestamp = timestamp;
    }

    /// <summary>
    ///     The received data deserialized into an object.
    /// </summary>
    public T Data { get; }

    /// <summary>
    ///     The timestamp the data was received.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    ///     Create a new DataEvent with data in the from of type K based on the current DataEvent. Topic, OriginalData and Timestamp will be copied over.
    /// </summary>
    /// <typeparam name="TData">The type of the new data.</typeparam>
    /// <param name="data">The new data.</param>
    /// <returns></returns>
    public DataEvent<TData> As<TData>(TData data)
    {
        return new DataEvent<TData>(data, Timestamp);
    }
}
