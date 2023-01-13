using System.Text.Json;
using DataHandler.Results;
using DataHandler.Results.Errors;

namespace DataHandler.Core;

public static class SystemTextJsonSerializer
{
    public static Result<T> DeserializeInternal<T>(this string json, JsonSerializerOptions? options = default)
    {
        try
        {
            return Result<T>.FromSuccess(JsonSerializer.Deserialize<T>(json, options)!);
        }
        catch (JsonException ex)
        {
            return new DeserializationError($"Deserialize JsonException: {ex.Message}");
        }
        catch (NotSupportedException ex)
        {
            return new DeserializationError($"Deserialize NotSupportedException: {ex.Message}");
        }
        catch (Exception ex)
        {
            return new DeserializationError($"Deserialize Unknown Exception: {ex.Message}");
        }
    }
}
