using KonkurLeft.Controllers;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDataProtection()
    .SetApplicationName($"javabak-{builder.Environment.EnvironmentName}")
    .PersistKeysToFileSystem(new DirectoryInfo($@"{builder.Environment.ContentRootPath}\keys"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

new UpdateController(builder.Environment, builder.Configuration).StartReminder();

app.Run();
