using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TalentCRM.Pages;

// Shown for "page not found" (404) and, outside Development, for unexpected errors.
// [ResponseCache] stops browsers from caching an error page.
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string Title { get; private set; } = "Something went wrong";
    public string Explanation { get; private set; } = "An unexpected error happened. Please try again.";
    public string? RequestId { get; private set; }

    // `code` comes from the query string, e.g. /Error?code=404 (set up in Program.cs).
    public void OnGet(int? code) => Describe(code);

    // Errors that happen during a form post are re-run as a POST, so handle that too.
    public void OnPost(int? code) => Describe(code);

    private void Describe(int? code)
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        // 📘 C#: switch statement with pattern matching on a nullable int.
        switch (code)
        {
            case 404:
                Title = "Page not found";
                Explanation = "That page, or the record you were looking for, doesn't exist. It may have been deleted.";
                break;
            case >= 400 and < 500:
                Title = "Request problem";
                Explanation = $"The request couldn't be completed (status {code}).";
                break;
        }
    }
}
