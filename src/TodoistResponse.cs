using System.Text.Json.Serialization;

public class TodoistResponse
{
    [JsonPropertyName("results")]
    public List<TodoistTask> Results { get; set; }

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }
}
