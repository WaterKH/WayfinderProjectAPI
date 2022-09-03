using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WayfinderProjectAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); ;

// Add DataBase
string databasePath = $"{builder.Environment.WebRootPath}/data/WayfinderProject.db";
Console.WriteLine(builder.Environment.WebRootPath);

builder.Services.AddDbContext<WayfinderContext>(options => options.UseSqlite($"Data Source={databasePath}"));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
