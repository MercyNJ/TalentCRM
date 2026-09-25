using Microsoft.AspNetCore.Mvc.Rendering;
using TalentCRM.Models;

namespace TalentCRM.Helpers;

// Builds option lists for select dropdowns.
public static class SelectLists
{
    // Generic method restricted to enum types.
    public static List<SelectListItem> For<TEnum>() where TEnum : struct, Enum =>
        Enum.GetValues<TEnum>()
            .Select(value => new SelectListItem(value.ToDisplay(), value.ToString()))
            .ToList();
}