﻿using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RestaurantAPI.Controllers;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;


namespace RestaurantAPI.IntegrationTestsMy;

public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Startup> _factory;

    public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
    {
        /*so we can configure our services : first we need to remove dbcontex registered for sqlServer and then add
         new service for inMemory Db*/

        _factory = factory  //watch out for this assignment must be like this 
      .WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextOptions =
                    services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                services.Remove(dbContextOptions);

                services.AddDbContext<RestaurantDbContext>(options =>
                {
                    options.UseInMemoryDatabase("RestaurantDb"); // this name is not important
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Theory]
    [InlineData("pageNumber=1&pageSize=5")]
    //[InlineData("pageNumber=2&pageSize=10")]
    public async Task GetAll_WithQueryParameters_ReturnsOKResult(string queryParameters)
    {
        //ACT
        var response = await _client.GetAsync("/api/restaurant?" + queryParameters);

        //ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("pageNumber=5&pageSize=4")]
    [InlineData("pageNumber=123&pageSize=1")]
    public async Task GetAll_WithInvalidQueryParameters_ReturnsBadRequest(string queryParameters)
    {
        //ACT
        var response = await _client.GetAsync("/api/restaurant?" + queryParameters);

        //ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    //Example of endpoint unit test basing emerson
    [Fact]
    public void Unit_test_of_controller_basing_emerson()
    {
        IRestaurantService service = Substitute.For<IRestaurantService>();

        PagedResult<RestaurantDto> pagedResult = default;

        service.GetAll(Arg.Any<RestaurantQuery>()).Returns(pagedResult);

        var restaurantController = new RestaurantController(service);

        var response =  restaurantController.GetAll(new RestaurantQuery());

        response.Result.Should().BeAssignableTo<OkObjectResult>();
    }

    //we test endpoint that creates new restaurant.
    [Fact]
    public async Task CreateRestaura_WithVaolidModel_Returns_CreatedStatus()
    {
        //Arrange
        var model = new CreateRestaurantDto()
        {
            Name = "TRestaurant",
            City = "Krakow",
            Street = "Dluga5 "
        };


        /*PostAsync requires httpContext parameter.
         StringContent inherits from httpContent so we cna use stringContent as httpContent
        we need to serialize out model to pass it as json to StringContent.
         */

        var json = JsonSerializer.Serialize(model);

        var httpContent = new StringContent(json, Encoding.UTF8,"application/json");

        //Act
        var response = await _client.PostAsync("/api/restaurant", httpContent);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        //we also check if server response contains header with location.
        response.Headers.Location.Should().NotBeNull();
    }
}