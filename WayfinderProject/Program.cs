using Blazored.Modal;
using Blazored.Toast;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Quartz;
using System.Text.Json.Serialization;
using WayfinderProject.Data.Jobs;
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

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Create a "key" for the job
    var jobKey = new JobKey("DailyCutsceneJob");

    // Register the job with the DI container
    q.AddJob<DailyCutsceneJob>(opts => opts.WithIdentity(jobKey));

    // Create a trigger for the job
    q.AddTrigger(opts => opts
        .ForJob(jobKey) // link to the HelloWorldJob
        .WithIdentity("DailyCutsceneJob-trigger") // give the trigger a unique name
        .WithCronSchedule("0 0 * * * ?")); // run every day at midnight
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

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
