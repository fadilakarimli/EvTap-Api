using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace EvTap.IntegrationTests;

/// <summary>
/// Exercises the showcase flow end to end against real infrastructure: create a listing,
/// approve it as Admin, and verify the async RabbitMQ consumer creates a Notification row for
/// a user whose saved search matches. Consumption happens off the request thread, so the
/// assertion polls with a bounded timeout rather than asserting immediately.
/// </summary>
[Collection(EvTapAppCollection.Name)]
public sealed class ListingApprovalNotificationTests(EvTapAppFixture fixture)
{
    [Fact]
    public async Task Approving_A_Listing_Notifies_Users_With_A_Matching_Saved_Search()
    {
        var client = fixture.HttpClient;
        var district = $"District-{Guid.NewGuid():N}"[..16];

        var ownerToken = await RegisterAndLoginAsync(client, "owner");
        var seekerToken = await RegisterAndLoginAsync(client, "seeker");

        await AuthorizedPostAsync(client, seekerToken, "/api/saved-searches", new
        {
            minPrice = 1000,
            maxPrice = 5000,
            minRooms = 2,
            district,
        });

        var createResponse = await AuthorizedPostAsync(client, ownerToken, "/api/listings", new
        {
            title = "Integration test listing",
            description = "Created by the integration test suite",
            price = 3000,
            rooms = 3,
            areaSquareMeters = 80,
            district,
            address = "Test address 1",
            latitude = 40.4,
            longitude = 49.8,
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var listingId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var adminToken = await LoginAsync(client, "admin@evtap.local", "Admin123!");

        var approveResponse = await AuthorizedPostAsync(
            client, adminToken, $"/api/listings/{listingId}/approve", body: null);

        Assert.Equal(HttpStatusCode.NoContent, approveResponse.StatusCode);

        var notification = await PollForMatchingNotificationAsync(client, seekerToken, listingId);

        Assert.NotNull(notification);
    }

    private static async Task<NotificationDto?> PollForMatchingNotificationAsync(
        HttpClient client,
        string token,
        Guid listingId)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);

        while (DateTime.UtcNow < deadline)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/notifications");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            var notifications = await response.Content.ReadFromJsonAsync<List<NotificationDto>>();

            var match = notifications?.FirstOrDefault(n => n.ListingId == listingId);
            if (match is not null)
            {
                return match;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        return null;
    }

    private static async Task<string> RegisterAndLoginAsync(HttpClient client, string prefix)
    {
        var email = $"{prefix}-{Guid.NewGuid():N}@example.com";
        const string password = "Password123!";

        var registerResponse = await client.PostAsJsonAsync("/api/users/register", new
        {
            name = prefix,
            email,
            password,
        });

        registerResponse.EnsureSuccessStatusCode();

        return await LoginAsync(client, email, password);
    }

    private static async Task<string> LoginAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/users/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.Token;
    }

    private static Task<HttpResponseMessage> AuthorizedPostAsync(
        HttpClient client,
        string token,
        string uri,
        object? body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = body is null ? null : JsonContent.Create(body),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client.SendAsync(request);
    }

    private sealed record LoginResponse(string Token, DateTime ExpiresAtUtc);

    private sealed record NotificationDto(Guid Id, Guid ListingId, string Channel, DateTime SentAt);
}
