// CREATIE VAN DE WEB APP ENVIRONMENT
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Data;
using ModelLibrary.Models;
using RealDolmenAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Om via de dbcontext data beheren
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();
UserController.Map(app);
BenchController.Map(app);

// SWAGGER ====== TOONT EEN SOORT PAGE MET DOCU VAN API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.Run();


