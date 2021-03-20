using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Webhooks.Api.Models;
using Webhooks.Application.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Webhooks.Api.Extensions
{
    /// <summary>
    /// Error handling configuration for the request pipeline
    /// </summary>
    public static class ExceptionHandling
    {
        /// <summary>
        /// Applies exception handling with different level of details depending on the environment
        /// </summary>
        /// <param name="app">Application builder</param>
        /// <param name="env">Environment</param>
        public static void UseExceptionHandling(this IApplicationBuilder app, IHostEnvironment env)
        {
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = feature.Error;

                context.Response.ContentType = "application/json";
                context.Response.Clear();

                if (exception is NotFoundException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                }
                else if (exception is DuplicateException)
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                }
                else if (exception is ValidationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    var validationException = (ValidationException)exception;
                    var response = new ValidationModel(
                        field: validationException.ValidationResult.MemberNames.First(),
                        message: validationException.ValidationResult.ErrorMessage);
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                }
                else
                {
                    var result = env.IsDevelopment() || env.IsLocal() ?
                        JsonConvert.SerializeObject(new ErrorModel
                        {
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                        }) :
                        JsonConvert.SerializeObject(new ErrorModel
                        {
                            Message = "An error occurred",
                            StackTrace = null
                        }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync(result);
                }
            }));
        }
    }
}
