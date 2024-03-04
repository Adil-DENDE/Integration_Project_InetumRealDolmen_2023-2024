// CREATIE VAN DE WEB APP ENVIRONMENT
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Data;
using ModelLibrary.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Om via de dbcontext data beheren
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

// SWAGGER ====== TOONT EEN SOORT PAGE MET DOCU VAN API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();


// ENDPOINT OM ALLE BENCHERS TE ZIEN //WE ZIEN NU NOG ALTIJD ALLE USERS (MANAGERS STAAN ER TUSSEN).
app.MapGet("/Benchers", async (AppDbContext db) =>  await db.User.ToListAsync());


// ENDPOINT ALS WE DE DATA VAN EEN BENCHER ZOUDEN WILLEN ZIEN 
app.MapGet("/Benchers/{id:int}", async (int id, AppDbContext db) => await db.User.FindAsync(id) is User user ? Results.Ok(user) : Results.NotFound());





app.Run();


