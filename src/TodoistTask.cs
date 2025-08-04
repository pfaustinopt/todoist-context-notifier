using System.Text.Json.Serialization;

public class TodoistTask
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("project_id")]
    public string ProjectId { get; set; }

    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; }

    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonPropertyName("checked")]
    public bool Checked { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("added_at")]
    public DateTime AddedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
