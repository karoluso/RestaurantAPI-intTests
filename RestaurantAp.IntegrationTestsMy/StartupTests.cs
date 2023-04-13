using AutoMapper.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using RestaurantAPI.Controllers;
using Microsoft.Extensions.DependencyInjection;


namespace RestaurantAPI.IntegrationTestsMy;

public class StartupTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _factory;
    private readonly IEnumerable<Type> _controllerTypes;
    public StartupTests(WebApplicationFactory<Startup> factory)
    {
        //we use reflection to get all types of controllers and register them as services.

        _controllerTypes = typeof(Startup)
            .Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ControllerBase)));

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                foreach (var type in _controllerTypes)
                {
                    services.AddScoped(type);
                }
            });
        });
    }

    [Fact]
    public void ConfigureServices_ForController_RegisteresAllDependiencies()
    {
        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory?.CreateScope();

        //we need a scope for controller
        foreach (var type in _controllerTypes)
        {
            var controller = scope?.ServiceProvider.GetService(type);

            controller.Should().NotBeNull();
        }
    }
}