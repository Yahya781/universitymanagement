using System;
using System.Collections.Generic;

namespace universitymanagementsystem.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public int CourseId { get; set; }

    public int? Semester { get; set; }

    public int? Year { get; set; }

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    public virtual Course Course { get; set; } = null!;

    public virtual Grade? Grade { get; set; }

    public virtual Student Student { get; set; } = null!;
}
