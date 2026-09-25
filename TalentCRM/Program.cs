using TalentCRM.Data;
using TalentCRM.Services;

// Configure the application.
var builder = WebApplication.CreateBuilder(args);

// Register Razor Pages and application services.
builder.Services.AddRazorPages();

string dataFile = Path.Combine(
    builder.Environment.ContentRootPath,
    builder.Configuration["TalentCRM:DataFile"] ?? "App_Data/talentcrm-data.json");

builder.Services.AddSingleton<ITalentStore>(sp =>
    new JsonFileTalentStore(
        dataFile,
        sp.GetRequiredService<ILogger<JsonFileTalentStore>>()));

builder.Services.AddScoped<CandidateService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<InterviewService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

await app.Services.GetRequiredService<ITalentStore>().LoadAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();