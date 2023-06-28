using InterviewTest.Cache;
using InterviewTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(option => { 
    option.AddServerHeader = false;
    
});
// Add services to the container.

builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<PersonContext>(opt => {
    opt.UseInMemoryDatabase("People");
    });
builder.Services.AddDbContext<PlaceContext>(opt => opt.UseInMemoryDatabase("Places"));
builder.Services.AddDbContext<ThingContext>(opt => opt.UseInMemoryDatabase("Things"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Remove Server response header


app.UseStaticFiles();
app.UseRouting();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;
app.Use((ctx, next) =>
{
    if (ctx.Response.Headers.ContainsKey("Server"))
    {
        ctx.Response.Headers.Remove("Server"); 
    }
    if (ctx.Response.Headers.ContainsKey("x-powered-by") || ctx.Response.Headers.ContainsKey("X-Powered-By"))
    {
        ctx.Response.Headers.Remove("x-powered-by");
        ctx.Response.Headers.Remove("X-Powered-By");
    }
    return next();
});
app.Run();
