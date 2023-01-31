using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Quartz;
using System.Text.Json.Serialization;
using WayfinderProject.Areas.Identity;
using WayfinderProject.Data;
using WayfinderProject.Data.Jobs;
using WayfinderProject.Data.Models;
using WayfinderProjectAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Add DataBase
var connectionString = Environment.GetEnvironmentVariable("ConnectionString");
var connection = new MySqlConnection(connectionString);
connection.Open();

builder.Services.AddDbContext<WayfinderContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b => b.SchemaBehavior(MySqlSchemaBehavior.Translate, (schema, entity) => $"{schema ?? "WFP"}_{entity}")), ServiceLifetime.Transient);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredToast();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDefaultIdentity<WayfinderProjectUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<WayfinderContext>();

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<WayfinderProjectUser>>();
builder.Services.AddScoped(x => new HttpClient() { BaseAddress = new Uri("https://localhost:49153/") });

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("GoogleAuthId") ?? "";
        options.ClientSecret = Environment.GetEnvironmentVariable("GoogleAuthSecret") ?? "";
    })
    .AddTwitter(twitterOptions =>
    {
        twitterOptions.ConsumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey");
        twitterOptions.ConsumerSecret = Environment.GetEnvironmentVariable("TwitterConsumerSecret");
        twitterOptions.RetrieveUserDetails = true;
    });


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Creation Daily Job
    var creationJobKey = new JobKey("DailyCreationJob");
    q.AddJob<DailyCreationJob>(opts => opts.WithIdentity(creationJobKey));

    q.AddTrigger(opts => opts
        .ForJob(creationJobKey)
        .WithIdentity("DailyCreationJob-trigger")
        .WithCronSchedule("0 0 * * * ?")); // run every day at midnight

    // Cutscene Daily Job
    var cutsceneJobKey = new JobKey("DailyCutsceneJob");
    q.AddJob<DailyCutsceneJob>(opts => opts.WithIdentity(cutsceneJobKey));

    q.AddTrigger(opts => opts
        .ForJob(cutsceneJobKey)
        .WithIdentity("DailyCutsceneJob-trigger")
        .WithCronSchedule("0 1 * * * ?")); // run every day at 1am

    // Entry Daily Job
    var entryJobKey = new JobKey("DailyEntryJob");
    q.AddJob<DailyEntryJob>(opts => opts.WithIdentity(entryJobKey));

    q.AddTrigger(opts => opts
        .ForJob(entryJobKey)
        .WithIdentity("DailyEntryJob-trigger")
        .WithCronSchedule("0 21 * * * ?")); // run every day at 9pm

    // Moogle Record Daily Job
    var moogleRecordJobKey = new JobKey("DailyMoogleRecordJob");
    q.AddJob<DailyMoogleRecordJob>(opts => opts.WithIdentity(moogleRecordJobKey));

    q.AddTrigger(opts => opts
        .ForJob(moogleRecordJobKey)
        .WithIdentity("DailyMoogleRecordJob-trigger")
        .WithCronSchedule("0 5 * * * ?")); // run every day at 5am
});

builder.Services.AddQuartzHostedService(opt =>
{
    opt.WaitForJobsToComplete = true;
});

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<WayfinderContext>();

//    DatabaseInitializer.Initialize(context);
//}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
