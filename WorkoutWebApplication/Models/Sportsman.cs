using System;
using System.Collections.Generic;

namespace WorkoutWebApplication.Models;

public partial class Sportsman
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public virtual ICollection<Subscription> Subscriptions { get; } = new List<Subscription>();
}
