using System.Text.Json.Serialization;

namespace DataHandler.Dummy.Models.Responses;

public record TradeResponse(
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("quantity")] decimal Quantity);
