using System;
using System.Collections.Generic;

namespace universitymanagementsystem.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public string RegistrationNo { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int? Semester { get; set; }

    public int DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Fee> Fees { get; set; } = new List<Fee>();

    public virtual User User { get; set; } = null!;
}
