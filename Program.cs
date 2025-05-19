using PowerBIDemo;

var builder = WebApplication.CreateBuilder(args);

// Binding PowerBI settings
builder.Services.Configure<PowerBISettings>(builder.Configuration.GetSection("PowerBI"));

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var settings = config.GetSection("PowerBI").Get<PowerBISettings>();
    return settings;
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<PowerBIService>();

var app = builder.Build();

app.MapGet("/api/powerbi/embed-token", async (PowerBIService powerBIService) =>
{
    var result = await powerBIService.GetEmbedInfoAsync();
    return Results.Ok(result);
});

app.Run();


