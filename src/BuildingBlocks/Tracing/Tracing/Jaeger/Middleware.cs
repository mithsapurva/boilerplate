﻿

namespace Tracing
{
    using Microsoft.AspNetCore.Http;
    using OpenTracing;
    using OpenTracing.Tag;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a class for Middleware
    /// </summary>
    internal sealed class Middleware
    {
        private readonly RequestDelegate _next;

        public Middleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITracer tracer)
        {
            IScope scope = null;
            var span = tracer.ActiveSpan;
            var method = context.Request.Method;

            if (span is null)
            {
                var spanBuilder = tracer.BuildSpan($"HTTP {method}");
                scope = spanBuilder.StartActive(true);
                span = scope.Span;
            }

            span.Log($"Processing HTTP {method}: {context.Request.Path}");

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                span.SetTag(Tags.Error, true);
                span.Log(ex.Message);
                throw;
            }
            finally
            {
                scope?.Dispose();
            }
        }
    }
}
