using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MovieCatalog.Domain.Interfaces;
using MovieCatalog.Infrastructure.Configuration;
using MovieCatalog.Infrastructure.Services;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace MovieCatalog.Infrastructure
{
    /// <summary>
    /// Provides extension methods to register infrastructure services with the DI container
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds infrastructure services to the service collection
        /// </summary>
        /// <param name="services">The service collection to add services to</param>
        /// <param name="configuration">The application configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register and configure TMDB options
            services.Configure<TmdbOptions>(options =>
            {
                // Required API key - throw if not configured
                options.ApiKey = configuration["TMDB:ApiKey"]
                    ?? throw new InvalidOperationException("TMDB API key is not configured");

                // Optional settings with fallbacks
                options.BaseUrl = configuration["TMDB:BaseUrl"] ?? "https://api.themoviedb.org/3";
                options.ImageBaseUrl = configuration["TMDB:ImageBaseUrl"] ?? "https://image.tmdb.org/t/p/";
                options.CacheExpirationMinutes = configuration.GetValue<int>("Caching:DefaultExpirationMinutes", 15);
            });

            // Add memory cache for API responses
            services.AddMemoryCache();

            // Register HTTP client with resilience policies
            services.AddHttpClient<IMovieService, TmdbApiService>(client =>
            {
                // Set default timeout to prevent hanging requests
                client.Timeout = TimeSpan.FromSeconds(30);
            })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    // Enable automatic decompression for better performance
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                })
                .AddPolicyHandler((services, request) => GetRetryPolicy(services.GetRequiredService<ILogger<TmdbApiService>>()))
                .AddPolicyHandler(GetTimeoutPolicy());

            return services;
        }

        /// <summary>
        /// Creates a retry policy for handling transient HTTP errors
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ILogger logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        logger.LogWarning(
                            "Retrying HTTP request ({Attempt} of 3) after {Delay}s delay due to {StatusCode}",
                            retryAttempt,
                            timespan.TotalSeconds,
                            outcome.Result?.StatusCode.ToString() ?? outcome.Exception?.Message);
                    });
        }

        /// <summary>
        /// Creates a timeout policy to prevent long-running requests
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                timeout: TimeSpan.FromSeconds(10),
                timeoutStrategy: TimeoutStrategy.Optimistic);
        }
    }
}
