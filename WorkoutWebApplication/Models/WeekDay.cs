using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutWebApplication.Models;

public partial class WeekDay
{
    public int Id { get; set; }
    [Display(Name = "День")]
    public string Name { get; set; } = null!;

    public virtual ICollection<PlansWorkout> PlansWorkouts { get; } = new List<PlansWorkout>();
}
