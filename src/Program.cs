using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Uwp.Notifications;
using Serilog;

public class Program
{
    private static readonly HttpClient client = new();
    private static TodoistConfig config;
    private static readonly string IconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "todoist.ico");
    private static string labelUrl;

    private static string GetTodoistFilterUrl(string label) =>
        $"https://app.todoist.com/app/label/{label}";

    static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/todoist-check-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 1)
            .CreateLogger();

        try
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.development.json", optional: true)
                .Build();

            config = new TodoistConfig
            {
                ApiKey = configuration["Todoist:ApiKey"] ?? throw new Exception("No API key found in settings"),
                Label = configuration["Todoist:Label"] ?? throw new Exception("No label in settings"),
            };

            // Store URL for activation handler
            labelUrl = GetTodoistFilterUrl(config.Label);

            // If this is a toast-activated instance, don't proceed with normal execution
            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                // Launch URL before exiting if we have it
                if (!string.IsNullOrEmpty(labelUrl))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = labelUrl,
                        UseShellExecute = true
                    });
                }
                return;
            }

            Log.Information("🔧 Starting Todoist label check script...");

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");

            var uri = $"https://todoist.com/api/v1/tasks?label={config.Label}";
            Log.Information($"🌐 Requesting: {uri}");

            try
            {
                var response = await client.GetStringAsync(uri);
                var todoistResponse = JsonSerializer.Deserialize<TodoistResponse>(response);
                var tasks = todoistResponse.Results;

                Log.Information($"✅ API call successful. Found {tasks.Count} task(s).");

                if (tasks.Any())
                {
                    Log.Information("\n🗒️ Matching tasks:");
                    foreach (var task in tasks)
                    {
                        Log.Information($" • [{task.Id}] {task.Content}");
                    }

                    // Show Windows Toast notification
                    var toastText = $"You have {tasks.Count} task(s) with label @{config.Label}.";
                    Log.Information($"\n🔔 Sending notification: {toastText}");

                    try
                    {
                        new ToastContentBuilder()
                            .AddText("Todoist Tasks")
                            .AddText(toastText)
                            .AddAppLogoOverride(new Uri(IconPath), ToastGenericAppLogoCrop.Circle)
                            .SetBackgroundActivation()
                            .Show(toast =>
                            {
                                toast.ExpirationTime = DateTime.Now.AddMinutes(1);
                                toast.Tag = "todoist-context-notifier";
                                toast.Group = "todoist";
                            });
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"⚠️ Failed to show notification: {ex.Message}");
                    }
                }
                else
                {
                    Log.Information($"ℹ️ No tasks found with label {config.Label}.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("❌ API request failed:");
                Log.Error(ex, "API request error");
                Environment.Exit(1);
            }

            Log.Information("🏁 Script complete.");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "❌ Fatal error occurred");
            Environment.Exit(1);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}