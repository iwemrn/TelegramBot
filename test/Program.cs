using Microsoft.EntityFrameworkCore;
using Serilog;
using test;
using test.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

builder.Services.Configure<AppSettings>(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<MessageSenderService>();

var app = builder.Build();

app.MapPost("/send", async (string botName, string message, MessageSenderService sender) =>
{
    await sender.SendMessageAsync(botName, message);
    return Results.Ok("Message sent (or attempted).");
});

app.Run();
