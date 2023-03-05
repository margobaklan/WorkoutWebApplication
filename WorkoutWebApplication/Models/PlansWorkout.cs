using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutWebApplication.Models;

public partial class PlansWorkout
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Тренування")]
    public int WorkoutId { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "День тижня")]
    public int WeekDayId { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "План")]
    public int PlanId { get; set; }

    [Display(Name = "План")]
    public virtual Plan Plan { get; set; } = null!;

    [Display(Name = "День")]
    public virtual WeekDay WeekDay { get; set; } = null!;

    [Display(Name = "Тренування")]
    public virtual Workout Workout { get; set; } = null!;
}
