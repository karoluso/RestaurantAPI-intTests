using System.IdentityModel.Tokens.Jwt;
using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.IntegrationTestsMy;

public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private HttpClient _httpClient;
    private IAccountService _accountService;

    public AccountControllerTests(WebApplicationFactory<Startup> factory)
    {
        _accountService = Substitute.For<IAccountService>(); 

        var configuredFactory = factory.WithWebHostBuilder(
            builder => builder.ConfigureServices(services =>
            {
                var dbContext = services.SingleOrDefault(x => 
                    x.ServiceType == typeof(DbContextOptions <RestaurantDbContext>));

                services.Remove(dbContext);

                services.AddDbContext<RestaurantDbContext>(options =>
                    options.UseInMemoryDatabase("RestaurantDb"));

                services.AddSingleton<IAccountService>(_accountService);
            }));

        _httpClient = configuredFactory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_For_valid_data_returns_Ok()
    {
        //Arrange
        var user = new RegisterUserDto()
        {
            Email = "blabla@op.pl",
            Password = "test123",
            ConfirmPassword = "test123"
        };

        var httpContent = user.ToJsonHttpContent();

        //Act
        var response = await _httpClient.PostAsync("api/account/register", httpContent);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterUser_For_NonValid_data_returns_BadRequest()
    {
        //Arrange
        var user = new RegisterUserDto()
        {
            Email = "invalidmail",
            Password = "test123",
            ConfirmPassword = "test123"
        };

        var httpContent = user.ToJsonHttpContent();

        //Act
        var response = await _httpClient.PostAsync("api/account/register", httpContent);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_returns_Ok_with_token()
    {
        //Arrange
        var loginDto = new LoginDto()
        {
            Email = "testEmial@op.pl",
            Password = "test123"
        };

        var sampleToken = "testToken";

        var tokenHandler = new JwtSecurityTokenHandler();

        _accountService.GenerateJwt(loginDto).Returns(sampleToken);

        var httpContent=loginDto.ToJsonHttpContent();

        //Act
        var response = await _httpClient.PostAsync("api/account/login", httpContent);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }
}