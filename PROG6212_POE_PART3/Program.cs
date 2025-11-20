using Microsoft.EntityFrameworkCore;

using PROG6212_POE_PART3.Data;
using PROG6212_POE_PART3.Models;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Session (already used in HomeController)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// EF Core with SQL Server Express
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

var app = builder.Build();

// Ensure database exists and seed some default users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

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

// Standard middleware
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
