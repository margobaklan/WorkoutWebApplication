using System;
using System.Collections.Generic;

namespace WorkoutWebApplication.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public int PlanId { get; set; }

    public int SportsmanId { get; set; }

    public DateTime Date { get; set; }

    public virtual Plan Plan { get; set; } = null!;

    public virtual Sportsman Sportsman { get; set; } = null!;
}
