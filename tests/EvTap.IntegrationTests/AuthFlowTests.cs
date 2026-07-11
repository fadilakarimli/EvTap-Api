using System.Net;
using System.Net.Http.Json;

namespace EvTap.IntegrationTests;

[Collection(EvTapAppCollection.Name)]
public sealed class AuthFlowTests(EvTapAppFixture fixture)
{
    [Fact]
    public async Task Register_Then_Login_Returns_Token()
    {
        var email = $"auth-{Guid.NewGuid():N}@example.com";

        var registerResponse = await fixture.HttpClient.PostAsJsonAsync("/api/users/register", new
        {
            name = "Integration Test User",
            email,
            password = "Password123!",
        });

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await fixture.HttpClient.PostAsJsonAsync("/api/users/login", new
        {
            email,
            password = "Password123!",
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var body = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrWhiteSpace(body?.Token));
    }

    [Fact]
    public async Task Login_With_Wrong_Password_Returns_Unauthorized()
    {
        var email = $"auth-{Guid.NewGuid():N}@example.com";

        await fixture.HttpClient.PostAsJsonAsync("/api/users/register", new
        {
            name = "Integration Test User",
            email,
            password = "Password123!",
        });

        var loginResponse = await fixture.HttpClient.PostAsJsonAsync("/api/users/login", new
        {
            email,
            password = "WrongPassword!",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }

    [Fact]
    public async Task Register_With_Duplicate_Email_Returns_Conflict()
    {
        var email = $"auth-{Guid.NewGuid():N}@example.com";

        var request = new { name = "First", email, password = "Password123!" };

        var first = await fixture.HttpClient.PostAsJsonAsync("/api/users/register", request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await fixture.HttpClient.PostAsJsonAsync("/api/users/register", request);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    private sealed record LoginResponse(string Token, DateTime ExpiresAtUtc);
}
