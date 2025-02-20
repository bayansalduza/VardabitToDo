using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Vardabit.API.Middlewares
{
    public static class GlobalExceptionMiddleware
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = feature?.Error;

                    context.Response.ContentType = "application/json";

                    var statusCode = (int)HttpStatusCode.InternalServerError;

                    var message = "Beklenmeyen bir hata oluştu.";

                    if (exception is ArgumentException)
                    {
                        statusCode = (int)HttpStatusCode.BadRequest;
                        message = exception.Message;
                    }
                    else if (exception is UnauthorizedAccessException)
                    {
                        statusCode = (int)HttpStatusCode.Unauthorized;
                        message = "Yetkisiz erişim.";
                    }

                    context.Response.StatusCode = statusCode;

                    object errorResponse;

                    if (env.IsDevelopment() && exception != null)
                    {
                        errorResponse = new
                        {
                            error = message,
                            details = exception.ToString() 
                        };
                    }
                    else
                    {
                        errorResponse = new { error = message };
                    }

                    await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                });
            });
        }
    }
}
