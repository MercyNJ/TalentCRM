using TalentCRM.Models;

namespace TalentCRM.Services;

internal static class StoreExtensions
{
    public static T GetOrThrow<T>(this List<T> items, int id) where T : Entity =>
        items.FirstOrDefault(x => x.Id == id)
        ?? throw new NotFoundException(typeof(T).Name, id);

    public static int NextId<T>(this List<T> items) where T : Entity =>
        items.Count == 0 ? 1 : items.Max(x => x.Id) + 1;

    public static bool ContainsText(this string? text, string search) =>
        text is not null && text.Contains(search, StringComparison.OrdinalIgnoreCase);
}
