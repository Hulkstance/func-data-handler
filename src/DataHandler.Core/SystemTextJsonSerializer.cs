using System.Text.Json;
using DataHandler.Results;
using DataHandler.Results.Errors;

namespace DataHandler.Core;

public static class SystemTextJsonSerializer
{
    public static Result<T> Deserialize<T>(this string json, JsonSerializerOptions? options = null)
    {
        try
        {
            return Result<T>.FromSuccess(JsonSerializer.Deserialize<T>(json, options)!);
        }
        catch (Exception ex)
        {
            return new DeserializationError($"Failed to deserialize: {ex.Message}");
        }
    }
}
