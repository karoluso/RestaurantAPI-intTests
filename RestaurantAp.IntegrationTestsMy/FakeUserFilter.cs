using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantAPI.IntegrationTestsMy;

public class FakeUserFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var claimsPrincipal = new ClaimsPrincipal();

        claimsPrincipal.AddIdentities(new[]
        {
            new ClaimsIdentity(
                new []
                {
                    new Claim(ClaimTypes.NameIdentifier,"1"),
                    new Claim(ClaimTypes.Role,"Admin"),
                })
        });

        await next(); // need to add this to continue processing this query;
    }
}