using FluentValidation;
using MassTransit;
using TransportService.Core.DTOs;
using TransportService.Core.Interfaces;
using TransportService.Core.Validators;
using TransportService.Infrastructure.Mapping;
using TransportService.Infrastructure.Repositories;
using TransportService.Infrastructure.Services;
using System.Reflection;

namespace TransportService.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add AutoMapper
        services.AddAutoMapper(typeof(TransportMappingProfile));

        // Add repositories
        services.AddScoped<ITransportRepository, TransportRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add services
        services.AddScoped<ITransportService, TransportService.Infrastructure.Services.TransportService>();

        // Add validators
        services.AddScoped<IValidator<CreateTransportRequest>, CreateTransportRequestValidator>();
        services.AddScoped<IValidator<UpdateTransportStatusRequest>, UpdateTransportStatusRequestValidator>();

        // Add FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateTransportRequestValidator)));

        return services;
    }

    public static IServiceCollection AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            // Configure RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMQSettings = configuration.GetSection("RabbitMQ");
                cfg.Host(rabbitMQSettings["Host"] ?? "localhost", h =>
                {
                    h.Username(rabbitMQSettings["Username"] ?? "guest");
                    h.Password(rabbitMQSettings["Password"] ?? "guest");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}