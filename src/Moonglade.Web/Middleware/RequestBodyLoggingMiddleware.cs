﻿using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using Moonglade.Data.Porting.Exporters;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Moonglade.Web.Middleware
{
    public class RequestBodyLoggingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var method = context.Request.Method;

            context.Request.EnableBuffering();

            if (context.Request.Body.CanRead && (method == HttpMethods.Post || method == HttpMethods.Put))
            {
                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 512, leaveOpen: true);

                var requestBody = await reader.ReadToEndAsync();

                context.Request.Body.Position = 0;

                var requestTelemetry = context.Features.Get<RequestTelemetry>();
                requestTelemetry?.Properties.Add("RequestBody", requestBody);

                if (context.User.Identity is { IsAuthenticated: true })
                {
                    var userName = context.User.Identity?.Name;
                    var email = context.User.FindFirst(p => p.Type == ClaimTypes.Email)?.Value;

                    requestTelemetry?.Properties.Add("MoongladeUser", JsonSerializer.Serialize(new
                    {
                        userName,
                        email
                    }, MoongladeJsonSerializerOptions.Default));
                }
            }

            await next(context);
        }
    }
}