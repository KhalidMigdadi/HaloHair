using System;
using System.Collections.Generic;

namespace HaloHair.Models;

public partial class JobApplication
{
    public int Id { get; set; }

    public int VacancyId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? CoverLetter { get; set; }

    public string? ResumeFilePath { get; set; }

    public string Status { get; set; } = null!;

    public DateTime AppliedAt { get; set; }

    public virtual Vacancy Vacancy { get; set; } = null!;
}
