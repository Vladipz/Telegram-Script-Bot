using Microsoft.EntityFrameworkCore;

using ScriptBot.API.Helpers.Interceptors;
using ScriptBot.BLL.Helpers;
using ScriptBot.BLL.Interfaces;
using ScriptBot.BLL.Services;
using ScriptBot.DAL.Data;

using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BotDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .AddInterceptors(sp.GetRequiredService<UptadeDatedEntityInterceptor>());
});

var botToken = builder.Configuration["TelegramBotToken"];

if (string.IsNullOrEmpty(botToken))
{
    throw new InvalidOperationException("TelegramBotToken is not set in configuration.");
}

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

builder.Services.AddSingleton<UptadeDatedEntityInterceptor>();

builder.Services.AddScoped<IUploadsService, UploadsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleManagerService, RoleManagerService>();
builder.Services.AddScoped<ITelegramService, TelegramService>();
builder.Services.AddScoped<ICommandService, CommandService>();
builder.Services.AddScoped<ISftpService, SftpService>();
builder.Services.AddScoped<IScriptGeneratorService, ScriptGeneratorService>();
builder.Services.ConfigureTelegramBotMvc();
builder.Services.AddControllers();

builder.Services.AddScoped<CommandRegistry>(); // Автоматична реєстрація команд
var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.MapControllers();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");

    // Enable buffering for request body
    context.Request.EnableBuffering();

    // Read the request body
    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    logger.LogInformation($"Request body: {body}");

    // Reset the request body position
    context.Request.Body.Position = 0;

    await next();
});
app.Run();