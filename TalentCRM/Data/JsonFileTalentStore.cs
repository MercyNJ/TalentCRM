using System.Text.Json;
using System.Text.Json.Serialization;

namespace TalentCRM.Data;

// Stores data in memory and persists it to a JSON file.
public class JsonFileTalentStore : ITalentStore
{
    private readonly string _filePath;
    private readonly ILogger<JsonFileTalentStore> _logger;
    private TalentData _data = new();

    // Allows asynchronous access to be handled one request at a time.
    private readonly SemaphoreSlim _gate = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    // ILogger<T> provides application logging.
    public JsonFileTalentStore(string filePath, ILogger<JsonFileTalentStore> logger)
    {
        _filePath = filePath;
        _logger = logger;
    }

    public async Task LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            _logger.LogInformation("No data file yet. Creating {File} with sample data.", _filePath);
            _data = SeedData.Create();
            await SaveAsync();
            return;
        }

        try
        {
            await using FileStream stream = File.OpenRead(_filePath);
            _data = await JsonSerializer.DeserializeAsync<TalentData>(stream, JsonOptions) ?? new TalentData();
            _logger.LogInformation("Loaded {Count} candidates from {File}.", _data.Candidates.Count, _filePath);
        }
        catch (JsonException ex)
        {
            string backup = $"{_filePath}.broken-{DateTime.Now:yyyyMMdd-HHmmss}";
            File.Move(_filePath, backup);
            _logger.LogWarning(ex, "Data file was not valid JSON. Moved it to {Backup} and started with sample data.", backup);
            _data = SeedData.Create();
            await SaveAsync();
        }
    }

    public T Read<T>(Func<TalentData, T> query)
    {
        _gate.Wait();
        try
        {
            return query(_data);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<T> WriteAsync<T>(Func<TalentData, T> change)
    {
        await _gate.WaitAsync();
        try
        {
            // Apply the change before saving.
            T result = change(_data);
            await SaveAsync();
            return result;
        }
        finally
        {
            _gate.Release();
        }
    }

    public Task WriteAsync(Action<TalentData> change) =>
        WriteAsync(data =>
        {
            change(data);
            return true;
        });

    // Writes to a temporary file before replacing the original.
    private async Task SaveAsync()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        string tempPath = _filePath + ".tmp";

        await using (FileStream stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, _data, JsonOptions);
        }

        File.Move(tempPath, _filePath, overwrite: true);
    }
}