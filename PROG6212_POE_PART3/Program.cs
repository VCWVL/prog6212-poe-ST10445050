using Microsoft.EntityFrameworkCore;
using PROG6212_POE_PART3.Data;
using PROG6212_POE_PART3.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC (Add support for controllers and views)
builder.Services.AddControllersWithViews();

// Session configuration for session management
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Check if we are in Test environment, and use In-Memory DB for tests
if (builder.Environment.IsEnvironment("Test"))
{
    // Use In-Memory Database for unit tests
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseInMemoryDatabase("TestDatabase"));
}
else
{
    // Use SQL Server for production
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
        ));
}

// Add controllers and views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ensure database creation and seeding of initial users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();  // Ensure the database is created and migrations are applied

    // Seed default users only if they don't already exist
    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User
            {
                Username = "hr1",
                Password = "123",
                Role = "HR",
                FirstName = "Hannah",
                LastName = "Resources",
                Email = "hr@cmcs.com",
                HourlyRate = 0
            },
            new User
            {
                Username = "lecturer1",
                Password = "123",
                Role = "Lecturer",
                FirstName = "Liam",
                LastName = "Lecturer",
                Email = "lecturer1@cmcs.com",
                HourlyRate = 500
            },
            new User
            {
                Username = "coordinator1",
                Password = "123",
                Role = "Coordinator",
                FirstName = "Cody",
                LastName = "Coordinator",
                Email = "coord1@cmcs.com",
                HourlyRate = 0
            },
            new User
            {
                Username = "manager1",
                Password = "123",
                Role = "Manager",
                FirstName = "Maya",
                LastName = "Manager",
                Email = "manager1@cmcs.com",
                HourlyRate = 0
            }
        );

        db.SaveChanges();
    }
}

// Configure the app for middleware (exception handling, HSTS, HTTPS, etc.)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Default route configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
