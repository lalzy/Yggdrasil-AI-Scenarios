// Program.cs

using Yggdrasil.Extensions;
using Yggdrasil.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo{
        Title = "Yggdrasil - AI Scenarios",
        Version = "v1",
    });
});

builder.Services.AddServices();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddControllers().AddJsonOptions(options =>{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
    
builder.Services.AddRazorPages().AddRazorPagesOptions(options => {
    options.RootDirectory = "/src/Pages";
});

var app = builder.Build();
app.Initialize();


app.MapControllers();

app.UseStaticFiles();
app.MapRazorPages();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
