using System.ComponentModel.DataAnnotations;

namespace PROG6212_POE_PART3.Models
{
    public class User
    {
        // Primary Key
        public int Id { get; set; }

        // Login details
        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Password { get; set; } = string.Empty; // plain for POE demo

        // Role: "Lecturer", "Coordinator", "Manager", "HR"
        [Required, StringLength(20)]
        public string Role { get; set; } = "Lecturer";

        // Personal info
        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Hourly rate for lecturers (0 for non-lecturer roles)
        [Range(0, 2000)]
        public double HourlyRate { get; set; }
    }
}




