using System;
using System.Collections.Generic;
namespace universitymanagementsystem.Models;
public partial class Fee
{
    public int FeeId { get; set; }
    public int StudentId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? PaidDate { get; set; }
    public string Status { get; set; } = null!;
    public string? Remarks { get; set; }
    public virtual Student Student { get; set; } = null!;
}