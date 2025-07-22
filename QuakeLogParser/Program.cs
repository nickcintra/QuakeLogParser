using QuakeLogParser.Core.Services;
using QuakeLogParser.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Adicionando dependências
builder.Services.AddScoped<IPlayerManager, PlayerManager>();
builder.Services.AddScoped<IKillUpdater, KillUpdater>();
builder.Services.AddScoped<ILogProcessor, KillEventProcessor>();
builder.Services.AddScoped<LogParserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
