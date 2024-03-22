using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RealDolmenAPI.Error
{
    public static class ErrorHandlingConfig
    {
        // Configureer globale foutafhandeling
        public static void UseGlobalErrorHandling(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                // Gebruik de Developer Exception Page voor gedetailleerde fouttracebacks
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Exception Handler Middleware voor foutafhandeling
                app.UseExceptionHandler(a => a.Run(async context =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    // een probleemdetails-object voor de foutrespons
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Er is een fout opgetreden.",
                        Status = 500,
                        Detail = exception?.Message
                    };

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                }));

                // Configureer StatusCodePages Middleware om een algemene foutinhoud te geven voor client- en serverfouten
                app.UseStatusCodePages(async context =>
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsJsonAsync(new { Error = "Er is een onverwachte fout opgetreden." });
                });
            }

            //test
            app.MapGet("/test-exception", () =>
            {
                throw new InvalidOperationException("Dit is een test uitzondering.");
            });
        }
    }
}
