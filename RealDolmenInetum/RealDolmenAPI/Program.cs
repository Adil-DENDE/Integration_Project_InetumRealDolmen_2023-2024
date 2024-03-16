// CREATIE VAN DE WEB APP ENVIRONMENT
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Data;
using ModelLibrary.Models;
using RealDolmenAPI.Controllers;
using RealDolmenAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBenchService, BenchService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Om via de dbcontext data beheren
builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});



var app = builder.Build();
UserController.Map(app);
BenchController.Map(app);

// SWAGGER ====== TOONT EEN SOORT PAGE MET DOCU VAN API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAnyOrigin");
app.UseHttpsRedirection();

app.Run();


