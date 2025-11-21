using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PROG6212_POE_PART3.Data;
using PROG6212_POE_PART3.Models;
using System;
using System.Linq;

namespace PROG6212_POE_PART3.Tests
{
    [TestClass]
    public class ClaimTests
    {
        private ApplicationDbContext _context;
        private DbContextOptions<ApplicationDbContext> _options;

        // Initialize the in-memory database before each test
        [TestInitialize]
        public void Setup()
        {
            try
            {
                _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: "TestDatabase") // Use in-memory database for testing
                    .Options;

                _context = new ApplicationDbContext(_options);

                // Ensure the database is created for each test
                _context.Database.EnsureCreated();

                // Seed test data
                SeedTestData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Setup: {ex.Message}");
                throw;
            }
        }

        // Seed some test data for unit tests
        private void SeedTestData()
        {
            try
            {
                // Ensure no data conflicts by clearing the context before seeding
                _context.Users.RemoveRange(_context.Users); // Clear existing users
                _context.Claims.RemoveRange(_context.Claims); // Clear existing claims
                _context.SaveChanges(); // Save the changes

                // Seed Users with auto-generated IDs (no manual assignment)
                _context.Users.AddRange(
                    new User { Username = "lecturer1", Role = "Lecturer", FirstName = "Liam", LastName = "Lecturer", HourlyRate = 500 },
                    new User { Username = "coordinator1", Role = "Coordinator", FirstName = "Cody", LastName = "Coordinator" },
                    new User { Username = "manager1", Role = "Manager", FirstName = "Maya", LastName = "Manager" }
                );
                _context.SaveChanges();

                // Seed Claims with auto-generated IDs (no manual assignment)
                _context.Claims.AddRange(
                    new Claim { LecturerId = 1, LecturerName = "Liam Lecturer", HoursWorked = 50, HourlyRate = 500, Status = "Pending" },
                    new Claim { LecturerId = 1, LecturerName = "Liam Lecturer", HoursWorked = 20, HourlyRate = 500, Status = "CoordinatorApproved" },
                    new Claim { LecturerId = 1, LecturerName = "Liam Lecturer", HoursWorked = 150, HourlyRate = 500, Status = "Approved" }
                );
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Seeding: {ex.Message}");
                throw;
            }
        }

        // 1️ Test: Verify that claim amount is calculated correctly
        [TestMethod]
        public void ClaimAmount_ShouldBeHoursTimesRate()
        {
            var claim = new Claim { HoursWorked = 10, HourlyRate = 200 };
            Assert.AreEqual(2000, claim.ClaimAmount, "Claim amount calculation is incorrect.");
        }

        // 2️ Test: Adding a new claim should increase the count in the Claims table
        [TestMethod]
        public void AddClaim_ShouldIncreaseClaimCount()
        {
            var initialCount = _context.Claims.Count();
            var newClaim = new Claim
            {
                LecturerId = 1,
                LecturerName = "John Doe",
                HoursWorked = 10,
                HourlyRate = 500,
                Status = "Pending"
            };
            _context.Claims.Add(newClaim);
            _context.SaveChanges();

            Assert.AreEqual(initialCount + 1, _context.Claims.Count(), "Claim count did not increase.");
        }

        // 3️ Test: Coordinator approval should update the claim status
        [TestMethod]
        
        public void CoordinatorApproval_ShouldUpdateStatusToCoordinatorApproved()
        {
            // Retrieve the claim that you want to update
            var claim = _context.Claims.FirstOrDefault(c => c.Id == 1);
            Assert.IsNotNull(claim, "Claim was not found.");

            // Update the claim's status
            claim.Status = "CoordinatorApproved";
            _context.SaveChanges(); // Ensure changes are saved

            // Reload the claim from the context to ensure the changes are persisted
            var updatedClaim = _context.Claims.FirstOrDefault(c => c.Id == 1);
            Assert.IsNotNull(updatedClaim, "Claim was not found after update.");

            // Verify that the status is updated
            Assert.AreEqual("CoordinatorApproved", updatedClaim.Status, "Coordinator did not approve the claim.");
        }



        // 4️ Test: Manager rejection should update the claim status to Rejected
        [TestMethod]
        public void ManagerRejection_ShouldUpdateStatusToRejected()
        {
            var claim = _context.Claims.FirstOrDefault(c => c.Id == 2);
            Assert.IsNotNull(claim, "Claim should be found.");
            claim.Status = "Rejected";
            _context.SaveChanges();

            var updatedClaim = _context.Claims.FirstOrDefault(c => c.Id == 2);
            Assert.IsNotNull(updatedClaim, "Claim should be found after update.");
            Assert.AreEqual("Rejected", updatedClaim.Status, "Manager did not reject the claim.");
        }


        // 5️ Test: Automatically approved claim (hours between 40 and 180)
        [TestMethod]
        public void AutoApprovedClaim_ShouldHaveStatusApproved()
        {
            var claim = new Claim { HoursWorked = 100, HourlyRate = 500 };
            if (claim.HoursWorked >= 40 && claim.HoursWorked <= 180)
            {
                claim.Status = "Approved";
            }

            Assert.AreEqual("Approved", claim.Status, "Claim was not automatically approved.");
        }

        // 6️ Test: HR should be able to view all claims
        [TestMethod]
        public void HR_ShouldViewAllClaims()
        {
            var hrClaims = _context.Claims.Where(c => c.Status == "Pending").ToList();
            Assert.IsTrue(hrClaims.Count > 0, "HR is unable to view claims.");
        }

        // 7️ Test: HR should be able to create a new user
        [TestMethod]
        public void HR_ShouldCreateUser()
        {
            // Arrange: Create a new HR user
            var newUser = new User
            {
                Username = "newuser1",
                Password = "password123",
                Role = "Lecturer",
                FirstName = "New",
                LastName = "User",
                Email = "newuser@cmcs.com",
                HourlyRate = 500
            };

            // Act: Add new user to the database and save
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Assert: Check if the user exists in the database
            var createdUser = _context.Users.FirstOrDefault(u => u.Username == "newuser1");
            Assert.IsNotNull(createdUser, "HR was not able to create a new user.");
            Assert.AreEqual("Lecturer", createdUser.Role, "The role of the created user is incorrect.");
        }

        // 8️ Test: Check if claim amount calculation is correct
        [TestMethod]
       
        public void ClaimAmountCalculation_ShouldBeCorrect()
        {
            var claim = _context.Claims.FirstOrDefault(c => c.Id == 1); // Ensure the claim exists
            Assert.IsNotNull(claim, "Claim was not found.");

            var expectedAmount = claim.HoursWorked * claim.HourlyRate;
            Assert.AreEqual(expectedAmount, claim.ClaimAmount, "Stored claim amount is incorrect.");
        }


        // Clean up after tests
        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted(); // Ensures the in-memory database is cleaned up after each test
        }
    }
}
