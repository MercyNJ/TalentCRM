namespace TalentCRM.Data;

// Defines the data-store contract.
// Func<TalentData, T> represents a function that takes TalentData and returns T.
public interface ITalentStore
{
    // Load the data.
    Task LoadAsync();

    // Read data without modifying it.
    T Read<T>(Func<TalentData, T> query);

    // Modify and save data.
    Task<T> WriteAsync<T>(Func<TalentData, T> change);

    // Modify and save data without returning a value.
    Task WriteAsync(Action<TalentData> change);
}