namespace gemini_wrapper;

using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.GenAI;
using Google.GenAI.Types;
using DotNetEnv;

public class EnemyResponseSchema {
  [JsonPropertyName("dialogue")]
  public string? Dialogue { get; set; }

  [JsonPropertyName("emotionalState")]
  public string? EmotionalState { get; set; }

  [JsonPropertyName("reasoning")]
  public string? Reasoning { get; set; }

  [JsonPropertyName("action")]
  public string? Action { get; set; }
}
public class GenerateContentSimpleText {
  public static async Task main() {
    
    DotNetEnv.Env.TraversePath().Load();
    string? apiKey = System.Environment.GetEnvironmentVariable("GOOGLE-API-KEY");

    var client = new Client(apiKey: apiKey);
    var response = await client.Models.GenerateContentAsync(
      model: "gemini-3-flash-preview", contents: SystemPrompt.Value + CharacterBackstory.Value + PlayerInput.Value
    );

    // Parse JSON response into schema
    string jsonText = response.Candidates[0].Content.Parts[0].Text;
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var schema = JsonSerializer.Deserialize<EnemyResponseSchema>(jsonText, options);

    // Output the structured response
    if (schema != null) {
      Console.WriteLine(JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true }));
    } else {
      Console.WriteLine("Failed to parse response");
    }
  }
}
