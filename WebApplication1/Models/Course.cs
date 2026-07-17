using System;
using System.Collections.Generic;

namespace universitymanagementsystem.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string? CourseCode { get; set; }

    public string CourseName { get; set; } = null!;

    public int CreditHours { get; set; }

    public int DepartmentId { get; set; }

    public int TeacherId { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Teacher Teacher { get; set; } = null!;
}
