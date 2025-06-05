using System;
                using System.Net;
                using System.Threading.Tasks;
                using Microsoft.AspNetCore.Http;
                using Microsoft.Extensions.Logging;
                using Newtonsoft.Json;
                using Volo.Abp;
                using Volo.Abp.AspNetCore.ExceptionHandling;
                using Volo.Abp.ExceptionHandling;
                using Volo.Abp.Http;

                namespace Mila.RoomBooking.Middleware
                {
                    public class GlobalExceptionHandlingMiddleware
                    {
                        private readonly RequestDelegate _next;
                        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
                        private readonly IExceptionToErrorInfoConverter _errorInfoConverter;

                        public GlobalExceptionHandlingMiddleware(
                            RequestDelegate next,
                            ILogger<GlobalExceptionHandlingMiddleware> logger,
                            IExceptionToErrorInfoConverter errorInfoConverter)
                        {
                            _next = next;
                            _logger = logger;
                            _errorInfoConverter = errorInfoConverter;
                        }

                        public async Task InvokeAsync(HttpContext context)
                        {
                            try
                            {
                                await _next(context);
                            }
                            catch (Exception ex)
                            {
                                await HandleExceptionAsync(context, ex);
                            }
                        }

                        private static RemoteServiceErrorInfo ConvertExceptionToErrorInfo(
                            IExceptionToErrorInfoConverter converter,
                            Exception exception)
                        {
                            return converter.Convert(exception, ConfigureErrorOptions);
                        }

                        private static void ConfigureErrorOptions(AbpExceptionHandlingOptions options)
                        {
                            // Removed invalid properties
                        }

                        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
                        {
                            var errorId = Guid.NewGuid().ToString();

                            _logger.LogError(exception, "Unhandled exception. Error ID: {ErrorId}", errorId);

                            var statusCode = HttpStatusCode.InternalServerError;
                            var errorInfo = ConvertExceptionToErrorInfo(_errorInfoConverter, exception);

                            // Add error ID to the response
                            if (errorInfo.Details == null)
                            {
                                errorInfo.Details = $"Error ID: {errorId}";
                            }
                            else
                            {
                                errorInfo.Details += $" | Error ID: {errorId}";
                            }

                            // Set appropriate status code for user-friendly exceptions
                            if (exception is UserFriendlyException)
                            {
                                statusCode = HttpStatusCode.BadRequest;
                            }

                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = (int)statusCode;

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorInfo));
                        }
                    }
                }
