using System.Text;
using System.Text.Json;

namespace RestaurantAPI.IntegrationTestsMy;

public static class TestHelper
{
    //static method that replace code duplication (commented ) in tests.
    public static StringContent ToJsonHttpContent(this object model)
    {
        var json = JsonSerializer.Serialize(model);

        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        return httpContent;
    }
}