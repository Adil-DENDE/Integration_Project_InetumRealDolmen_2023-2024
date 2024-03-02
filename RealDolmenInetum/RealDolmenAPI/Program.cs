// CREATIE VAN DE WEB APP ENVIRONMENT
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// SWAGGER ====== TOONT EEN SOORT PAGE MET DOCU VAN API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// ENDPOINT OM TE TESTEN
app.MapGet("/User", () => "THIS IS A TEST");
app.MapPost("/User", () => "THIS IS A TEST");
app.MapPut("/User", () => "THIS IS A TEST");
app.MapDelete("/User", () => "THIS IS A TEST");








app.Run();


