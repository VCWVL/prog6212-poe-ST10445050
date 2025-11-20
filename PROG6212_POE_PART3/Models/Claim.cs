using PROG6212_POE_PART3.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_POE_PART3.Models
{
    public class Claim
    {
        // Primary Key
        public int Id { get; set; }

        // Link to lecturer (User)
        [Required]
        public int LecturerId { get; set; }

        [ForeignKey(nameof(LecturerId))]
        public User? Lecturer { get; set; }

        // Lecturer name for display (auto-filled, not edited by lecturer)
        [Required]
        public string LecturerName { get; set; } = string.Empty;

        // Number of hours worked, required and must be between 1 and 180 (per Part 3)
        [Required(ErrorMessage = "Hours worked is required.")]
        [Range(1, 180, ErrorMessage = "Hours worked must be between 1 and 180.")]
        public double HoursWorked { get; set; }

        // Hourly rate, auto-filled from User
        [Required(ErrorMessage = "Hourly rate is required.")]
        [Range(100, 2000, ErrorMessage = "Hourly rate must be between 100 and 2000.")]
        public double HourlyRate { get; set; }

        // Calculated property: automatically computes total claim amount
        [NotMapped]
        public double ClaimAmount => HoursWorked * HourlyRate;

        // Stored total in database for reporting (optional but handy)
        public double StoredClaimAmount { get; set; }

        // Optional notes field, maximum 200 characters
        [StringLength(200)]
        public string? Notes { get; set; }

        // Status of the claim
        public string Status { get; set; } = "Pending";

        // Date submitted (for HR reports)
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        // Encrypted supporting document file name (if uploaded)
        public string? SupportingDocument { get; set; }  // No Required attribute, making it optional

        // Original file name of the supporting document for display/download purposes
        public string? OriginalFileName { get; set; }
    }
}
