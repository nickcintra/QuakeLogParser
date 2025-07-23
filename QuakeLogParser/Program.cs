using QuakeLogParser.Core.Services;
using QuakeLogParser.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Adicionando depend�ncias
builder.Services.AddScoped<IPlayerManager, PlayerManager>();
builder.Services.AddScoped<IKillUpdater, KillUpdater>();
builder.Services.AddScoped<ILogProcessor, KillEventProcessor>();
builder.Services.AddScoped<ILogProcessor, PlayerConnectionProcessor>();
builder.Services.AddScoped<ILogReader, LogReader>();
builder.Services.AddScoped<LogParserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
