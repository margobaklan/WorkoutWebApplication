using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutWebApplication.Models;

public partial class Plan
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "План")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Опис")]
    public string Description { get; set; } = null!;

    public virtual ICollection<PlansWorkout> PlansWorkouts { get; } = new List<PlansWorkout>();

    public virtual ICollection<Subscription> Subscriptions { get; } = new List<Subscription>();
}
