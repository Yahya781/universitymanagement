using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace universitymanagementsystem.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int UserId { get; set; }
    
    [Required]
    [Display(Name = "Registration Number")]
    [RegularExpression(@"^S-\d{5}$",
    ErrorMessage = "Registration Number must be in format S-24001.")]
    public string RegistrationNo { get; set; } = null!;

    [Required]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required]
    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    [RegularExpression(@"^03\d{9}$",
    ErrorMessage = "Phone number must start with 03 and contain exactly 11 digits.")]
    public string? Phone { get; set; }

    [Required]
    [StringLength(150)]
    public string? Address { get; set; }

    [Required]
    [Range(1, 8, ErrorMessage = "Semester must be between 1 and 8.")]
    public int? Semester { get; set; }

    [Required]
    [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual User User { get; set; } = null!;
}
