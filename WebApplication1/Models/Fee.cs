using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace universitymanagementsystem.Models
{
    public class Fee
    {
        [Key]
        public int FeeId { get; set; }

        public int StudentId { get; set; }

        public decimal Amount { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        public string Status { get; set; }

        public string? Remarks { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}