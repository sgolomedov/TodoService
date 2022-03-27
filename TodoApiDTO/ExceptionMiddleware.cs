using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TodoApiDTO.Services;

namespace TodoApi
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _isDevelopment;

        public ExceptionMiddleware(RequestDelegate next, bool isDevelopment)
        {
            _next = next;
            _isDevelopment = isDevelopment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (TodoItemNotFoundException notFoundException)
            {
                await HandleException(httpContext, notFoundException, HttpStatusCode.NotFound);
            }
            catch (ValidationException validationException)
            {
                await HandleException(httpContext, validationException, HttpStatusCode.BadRequest);
            }
        }
        private async Task HandleException(HttpContext context, Exception exception, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = code,
                Message = _isDevelopment ? exception.Message : "Server Error"
            }));
        }
    }
}
