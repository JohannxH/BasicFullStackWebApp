using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Users = Set<User>();
        Tokens = Set<Token>();
        //Roles = Set<Role>();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    // public DbSet<Role> Roles { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Add a test user
        // Seed data if necessary
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "testuser", Password = BCrypt.Net.BCrypt.HashPassword("password"), Role = "Admin" },
            new User { Id = 2, Username = "newuser", Password = BCrypt.Net.BCrypt.HashPassword("newpassword"), Role = "User" }
        );

        //// Add basic roles
        //modelBuilder.Entity<Role>().HasData(
        //    new Role { Id = 1, Type = "Admin" },
        //    new Role { Id = 2, Type = "User" }
        //);
    }
}