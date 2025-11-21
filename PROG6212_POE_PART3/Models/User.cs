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





/*
Development Details
Name: Keona Mackan
Student Number: ST10445050
Module: PROG6212
Assessment Type: POE Part Three

Reference List:

bulltorious, 2011. Applying jQuery UI CSS to MVC .input-validation-error. [online] Stack Overflow. Available at: <https://stackoverflow.com/questions/4979626/applying-jquery-ui-css-to-mvc-input-validation-error> 
[Accessed 17 September 2025].

Loupins, 2014. How to add an image? [online] Stack Overflow. Available at: <https://stackoverflow.com/questions/25142900/how-to-add-an-image> 
[Accessed 15 September 2025].

ASP.NET, 2009. How do I apply a CSS class to Html.ActionLink in ASP.NET MVC? [online] Stack Overflow. Available at: <https://stackoverflow.com/questions/1444495/how-do-i-apply-a-css-class-to-html-actionlink-in-asp-net-mvc> 
[Accessed 16 September 2025].

NetSecProf. 2022. C# Unit Testing using MSTest Test Projects in Visual Studio. [online] YouTube. Available at:<https://www.youtube.com/watch?v=icZwIKFrN84>
[Accessed 22 October 2025].

‌
*/