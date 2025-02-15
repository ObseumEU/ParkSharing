using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Obseum.Telemetry;
using OpenTelemetry;
using System.Reflection;
using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.Hosting;
using Sentry.OpenTelemetry;

namespace Microsoft.Extensions.Hosting
{
    // Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
    // This project should be referenced by each service project in your solution.
    // To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
    public static class Extensions
    {
        public static IHostApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddEnvironmentVariables();
            builder.Services.AddFeatureManagement();
            //builder.ConfigureOpenTelemetry();
            builder.AddDefaultHealthChecks();

            Console.WriteLine("OpenTelemetry: ");
            builder.AddOpenTelemetryExporters();
            builder.Services.AddServiceDiscovery();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });

            return builder;
        }

        public static IHostApplicationBuilder ConfigureMassTransit(this IHostApplicationBuilder builder, string host, Assembly consumersAssembly)
        {
            var consumers = consumersAssembly.GetTypes()
                                    .Where(t => t.GetInterfaces()
                                                 .Any(i => i.IsGenericType &&
                                                           i.GetGenericTypeDefinition() == typeof(IConsumer<>)))
                                    .ToArray();

            builder.Services.AddMassTransit(x =>
            {
                foreach (var consumer in consumers)
                {
                    x.AddConsumer(consumer);
                }
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(host);
                    cfg.ConfigureEndpoints(context);
                    cfg.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Interval(20, TimeSpan.FromSeconds(5));
                    });
                });
            });
            return builder;
        }

        private static WebApplicationBuilder AddOpenTelemetryExporters(this WebApplicationBuilder builder)
        {
            builder.AddObservability(configureTracing: (tracingBuilder) =>
            {
                tracingBuilder.AddSentry();
            });

            builder.WebHost.UseSentry(options =>
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
                options.TracesSampleRate = 1.0;
                options.UseOpenTelemetry();
            });

            return builder;
        }

        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Adding health checks endpoints to applications in non-development environments has security implications.
            // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });

            return app;
        }
    }
}
