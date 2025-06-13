
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RepFabric.Api.Extensions
{

    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var error = exceptionHandlerPathFeature?.Error;

                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(error, "An unhandled exception occurred.");

                    var errorResponse = new
                    {
                        message = "An unexpected error occurred. Please try again later."
                        #if DEBUG
                        ,
                        detail = error?.Message
                        #endif
                    };

                    await context.Response.WriteAsJsonAsync(errorResponse);
                });
            });
        }
    }
}
