using System;
using System.Collections.Generic;

namespace universitymanagementsystem.Models;

public partial class Grade
{
    public int GradeId { get; set; }

    public int EnrollmentId { get; set; }

    public decimal? QuizMarks { get; set; }

    public decimal? AssignmentMarks { get; set; }

    public decimal? MidMarks { get; set; }

    public decimal? FinalMarks { get; set; }

    public decimal? TotalMarks { get; set; }

    public string? LetterGrade { get; set; }

    public virtual Enrollment Enrollment { get; set; } = null!;
}
