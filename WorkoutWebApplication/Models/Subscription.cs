using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutWebApplication.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public int PlanId { get; set; }

    public string SportsmanId { get; set; }
    //public string UserId { get; set; }
    [Display(Name = "Дата")]
    public DateTime Date { get; set; }
    public string UserId { get; set; }
    [Display(Name = "План")]

    public virtual Plan Plan { get; set; } = null!;
    [Display(Name = "Спорстмен")]

    public virtual Sportsman Sportsman { get; set; } = null!;
}
