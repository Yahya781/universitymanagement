using System.ComponentModel.DataAnnotations;

namespace universitymanagementsystem.Models
{
    public partial class Attendance
    {
        public int AttendanceId { get; set; }

        public int EnrollmentId { get; set; }

        public DateOnly AttendanceDate { get; set; }

        public string? Status { get; set; }

        public virtual Enrollment Enrollment { get; set; } = null!;
    }

    public class AttendancePickerViewModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly AttendanceDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    }

    public class AttendanceRowViewModel
    {
        public int EnrollmentId { get; set; }
        public string RegistrationNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Status { get; set; } = "Present";
    }

    public class MarkAttendanceViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public DateOnly AttendanceDate { get; set; }
        public List<AttendanceRowViewModel> Students { get; set; } = new();
    }
}