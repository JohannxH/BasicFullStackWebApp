using Microsoft.VisualStudio.TestPlatform.TestHost;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_ReturnsSuccess()
    {
        var newUser = new UserLogin { Username = "testuser2", Password = "password2" };
        var response = await _client.PostAsJsonAsync("/api/auth/register", newUser);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("User registered successfully.", content);
    }

    [Fact]
    public async Task LoginUser_ReturnsToken()
    {
        var userLogin = new UserLogin { Username = "testuser", Password = "password" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", userLogin);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsAsync<dynamic>();
        Assert.NotNull(content.token);
    }

    [Fact]
    public async Task GetProtectedData_ReturnsData()
    {
        var userLogin = new UserLogin { Username = "testuser", Password = "password" };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", userLogin);
        loginResponse.EnsureSuccessStatusCode();

        var loginContent = await loginResponse.Content.ReadAsAsync<dynamic>();
        var token = loginContent.token;

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", (string)token);
        var response = await _client.GetAsync("/api/auth/protected-data");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("This is protected data.", content);
    }
}
