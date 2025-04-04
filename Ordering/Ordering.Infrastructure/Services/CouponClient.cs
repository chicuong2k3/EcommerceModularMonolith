using System.Net.Http.Json;
using System.Text.Json;

namespace Ordering.Infrastructure.Services;

public class CouponClient : ICouponService
{
    private readonly HttpClient httpClient;
    public CouponClient(IHttpClientFactory httpClientFactory)
    {
        httpClient = httpClientFactory.CreateClient("PromotionService");
    }

    public async Task<Money?> ApplyCouponAsync(OrderDetails orderDetails, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/coupons/apply", JsonSerializer.Serialize(orderDetails), cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Money>(content);
        }

        return null;
    }
}
