using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;

namespace RestaurantAPI.IntegrationTestsMy.ValidatorTests;

public class RegisterUserDtoValidatorTests
{
    private readonly RegisterUserDtoValidator _validator;
    private readonly RestaurantDbContext _dbContext;

    public RegisterUserDtoValidatorTests()
    {
        var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
        builder.UseInMemoryDatabase("TestDb");
        _dbContext = new RestaurantDbContext(builder.Options);
        _validator = new RegisterUserDtoValidator(_dbContext);

        SeedDb();
    }

    public void SeedDb()
    {
        _dbContext.Users.Add(new() { Email = "test123@op.pl" });
        _dbContext.SaveChanges();
    }

    public static IEnumerable<object[]> GetSampleInValidData()
    {
        yield return new object[] { new RegisterUserDto() { Email = "test123@op.pl", Password = "test123", ConfirmPassword = "test123" }, nameof(RegisterUserDto.Email) };
        yield return new object[] { new RegisterUserDto() { Email = "test123@op.pl", Password = "test123", ConfirmPassword = "test122" }, nameof(RegisterUserDto.ConfirmPassword) };
    }

    [Fact]
    public void Validator_for_valid_dto_returns_success()
    {
        //Arrange
        var registerUserDto = new RegisterUserDto()
        {
            Email = "test121@op.pl",
            Password = "test123",
            ConfirmPassword = "test123"
        };

        //Act
        var result = _validator.TestValidate(registerUserDto);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(GetSampleInValidData))]
    public void Validator_for_inValid_dto_returns_error(RegisterUserDto dto, string propertyName)
    {
     //Act
        var result=_validator.TestValidate(dto);

        //Assert
        result.ShouldHaveValidationErrorFor(propertyName);
    }

}