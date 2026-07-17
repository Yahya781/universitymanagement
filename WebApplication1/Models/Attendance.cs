using System;
using System.Collections.Generic;

namespace universitymanagementsystem.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int EnrollmentId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public string? Status { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;
}
