using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutWebApplication.Models;

public partial class Workout
{
    public int Id { get; set; }
    [Required(ErrorMessage ="Поле не повинно бути порожнім")]
    [Display(Name="Тренування")]
    public string Name { get; set; } = null!;
    public int Faid { get; set; }

    public int Wtid { get; set; }
    [Display(Name = "Тривалість")]
    public int Duration { get; set; }
    [Display(Name = "Обладнання")]
    public bool Equipment { get; set; }
    [Display(Name = "Фокус")]
    public virtual FocusArea Fa { get; set; } = null!;

    public virtual ICollection<PlansWorkout> PlansWorkouts { get; } = new List<PlansWorkout>();
    [Display(Name = "Тип")]
    public virtual WorkoutType Wt { get; set; } = null!;
}
