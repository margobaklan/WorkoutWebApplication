using System;
using System.Collections.Generic;

namespace WorkoutWebApplication.Models;

public partial class FocusArea
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Workout> Workouts { get; } = new List<Workout>();
}
