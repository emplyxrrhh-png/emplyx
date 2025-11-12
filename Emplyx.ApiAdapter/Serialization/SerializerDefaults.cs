using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emplyx.ApiAdapter.Serialization;

internal static class SerializerDefaults
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    static SerializerDefaults()
    {
        Options.Converters.Add(new JsonStringEnumConverter());
    }
}
