using Microsoft.VisualStudio.TestPlatform.TestHost; // Adjust the namespace to match where User is defined

public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public UserControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUser_ReturnsUser()
    {
        var response = await _client.GetAsync("/api/users/1");
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadAsAsync<User>();
        Assert.Equal("testuser", user.Username);
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedUser()
    {
        var newUser = new User { Username = "newuser", Password = "newpassword", Role = "User" };
        var response = await _client.PostAsJsonAsync("/api/users", newUser);
        response.EnsureSuccessStatusCode();

        var createdUser = await response.Content.ReadAsAsync<User>();
        Assert.Equal("newuser", createdUser.Username);
    }
}
