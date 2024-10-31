using Microsoft.EntityFrameworkCore;

public class ApplicationDbContextTests
{
    private DbContextOptions<ApplicationDbContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "BasicFullStackWebAppDb")
            .EnableSensitiveDataLogging()
            .Options;
    }

    [Fact]
    public void CanAddAndRetrieveUser()
    {
        var options = GetInMemoryOptions();

        // Insert seed data into the database using one instance of the context
        using (var context = new ApplicationDbContext(options))
        {
            context.Users.Add(new User { Username = "newuser", Password = "newpassword", Role = "User" });
            context.SaveChanges();
        }

        // Use a separate instance of the context to verify correct data was saved to the database
        using (var context = new ApplicationDbContext(options))
        {
            var user = context.Users.SingleOrDefault(u => u.Username == "newuser");
            Assert.NotNull(user);
            Assert.Equal("newuser", user.Username);
        }
    }

    [Fact]
    public void TestUserIsSeeded()
    {
        var options = GetInMemoryOptions();

        // Use a separate instance of the context to verify the test user is seeded
        using (var context = new ApplicationDbContext(options))
        {
            var user = context.Users.SingleOrDefault(u => u.Username == "testuser");
            Assert.NotNull(user);
            //Assert.Equal("testuser", user.Username);
            //Assert.Equal("password", user.Password);
        }
    }

    [Fact]
    public void CanAddAndRetrieveToken()
    {
        var options = GetInMemoryOptions();

        // Insert seed data into the database using one instance of the context
        using (var context = new ApplicationDbContext(options))
        {
            context.Tokens.Add(new Token { Value = "testtoken" });
            context.SaveChanges();
        }

        // Use a separate instance of the context to verify correct data was saved to the database
        using (var context = new ApplicationDbContext(options))
        {
            var token = context.Tokens.SingleOrDefault(t => t.Value == "testtoken");
            Assert.NotNull(token);
            Assert.Equal("testtoken", token.Value);
        }
    }

    [Fact]
    public void CanUpdateUser()
    {
        var options = GetInMemoryOptions();

        // Insert seed data into the database using one instance of the context
        using (var context = new ApplicationDbContext(options))
        {
            var user = new User { Username = "updateuser", Password = "oldpassword", Role = "User" };
            context.Users.Add(user);
            context.SaveChanges();

            // Update the user's password
            user.Password = "newpassword";
            context.Users.Update(user);
            context.SaveChanges();
        }

        // Use a separate instance of the context to verify the user's password was updated
        using (var context = new ApplicationDbContext(options))
        {
            var user = context.Users.SingleOrDefault(u => u.Username == "updateuser");
            Assert.NotNull(user);
            Assert.Equal("newpassword", user.Password);
        }
    }

    [Fact]
    public void CanDeleteUser()
    {
        var options = GetInMemoryOptions();

        // Insert seed data into the database using one instance of the context
        using (var context = new ApplicationDbContext(options))
        {
            var user = new User { Username = "deleteuser", Password = "password", Role = "User" };
            context.Users.Add(user);
            context.SaveChanges();

            // Delete the user
            context.Users.Remove(user);
            context.SaveChanges();
        }

        // Use a separate instance of the context to verify the user was deleted
        using (var context = new ApplicationDbContext(options))
        {
            var user = context.Users.SingleOrDefault(u => u.Username == "deleteuser");
            Assert.Null(user);
        }
    }
}
