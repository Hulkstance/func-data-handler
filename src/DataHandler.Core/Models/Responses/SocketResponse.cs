using System.Text.Json.Serialization;

namespace DataHandler.Core.Models.Responses;

public record SocketResponse<T>(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("channel")] string Channel,
    [property: JsonPropertyName("data")] T Data);
