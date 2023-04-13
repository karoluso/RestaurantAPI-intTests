using FluentValidation.TestHelper;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;

namespace RestaurantAPI.IntegrationTestsMy.ValidatorTests;

public class RestaurantQueryValidatorTests
{
    private readonly RestaurantQueryValidator _validator;

    public RestaurantQueryValidatorTests()
    {
        _validator = new RestaurantQueryValidator();
    }

    public static IEnumerable<object[]> GetSampleValidData()
    {
        yield return new object[] { new RestaurantQuery() { PageNumber = 1, PageSize = 15 } };
        yield return new object[] { new RestaurantQuery() { PageNumber = 2, PageSize = 5 } };
        yield return new object[] { new RestaurantQuery() { PageNumber = 1, PageSize = 10 } };
    }

    public static IEnumerable<object[]> GetSampleInValidData()
    {
        yield return new object[] { new RestaurantQuery() { PageNumber = 0, PageSize = 15 }, nameof(RestaurantQuery.PageNumber) };
        yield return new object[] { new RestaurantQuery() { PageNumber = 2, PageSize = 3 }, nameof(RestaurantQuery.PageSize) };
    }

    [Theory]
    [MemberData(nameof(GetSampleValidData))]
    public void Validator_for_valid_object_should_give_success(RestaurantQuery query)
    {
        //Act
        var result = _validator.TestValidate(query);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(GetSampleInValidData))]
    public void Validator_for_inValid_PageSize_returns_Error(RestaurantQuery query, string propertyName)
    {
        //Act
        var result = _validator.TestValidate(query);

        //Assert
        result.ShouldHaveValidationErrorFor(propertyName);
    }

}