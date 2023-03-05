using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkoutWebApplication.Models;

public partial class Sportsman
{
    public string Id { get; set; }
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [RegularExpression("^([a-zA-Z\u0400-\u04FF]{1,})$", ErrorMessage = "Використайте лише символи (A-Я) (a-я) (A-Z) (a-z) (' -)")]
    [Display(Name = "Ім'я")]
    public string FirstName { get; set; }// = null!;
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [RegularExpression("^([a-zA-Z\u0400-\u04FF]{1,}'?-?([a-zA-Z\u0400-\u04FF]{1,})?\\s?([a-zA-Z\u0400-\u04FF]{1,})?)$", ErrorMessage = "Використайте лише символи (A-Я) (a-я) (A-Z) (a-z) (' -)")]
    [Display(Name = "Прізвище")]
    public string Surname { get; set; }// = null!;

    public virtual ICollection<Subscription> Subscriptions { get; } = new List<Subscription>();
}
