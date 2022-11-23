using Blazored.Modal;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Text.Json.Serialization;
using WayfinderProject.Data;
using WayfinderProjectAPI.Data;
using Microsoft.AspNetCore.Identity;
using WayfinderProject.Data.Models;
using Microsoft.AspNetCore.Components.Authorization;
using WayfinderProject.Areas.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Blazored.Toast;

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

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

builder.Services.AddAuthentication()
   //.AddGoogle(options =>
   //{
   //    options.ClientId = Environment.GetEnvironmentVariable("GoogleAuthId");
   //    options.ClientSecret = Environment.GetEnvironmentVariable("GoogleAuthSecret");
   //})
   //.AddTwitter(twitterOptions =>
   //{
   //    twitterOptions.ConsumerKey = Environment.GetEnvironmentVariable("TwitterAuthId");
   //    twitterOptions.ConsumerSecret = Environment.GetEnvironmentVariable("TwitterAuthSecret");
   //    twitterOptions.RetrieveUserDetails = true;
   //})
   .AddOAuth("GitHub", "GitHub", githubOptions =>
   {
       githubOptions.ClientId = Environment.GetEnvironmentVariable("GitHubAuthId");
       githubOptions.ClientSecret = Environment.GetEnvironmentVariable("GitHubAuthSecret");
       githubOptions.CallbackPath = "/";
       githubOptions.AuthorizationEndpoint = "/";
       githubOptions.TokenEndpoint = "/";
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
