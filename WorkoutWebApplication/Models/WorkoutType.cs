using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutWebApplication.Models;

public partial class WorkoutType
{
    public int Id { get; set; }
    [Display(Name = "Тип")]
    public string Name { get; set; } = null!;

    public virtual ICollection<Workout> Workouts { get; } = new List<Workout>();
}
