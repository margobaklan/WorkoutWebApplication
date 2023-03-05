using System.ComponentModel.DataAnnotations;

namespace WorkoutWebApplication.ViewModel
{
    public class RegisterViewModel
    {

        [Display(Name ="Email")]
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Рік народження")]
        public int Year { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Поле не повинно бути порожнім")]
        [Compare("Password", ErrorMessage ="Паролі не співпадають")]
        [Display(Name = "Підтвердження паролю")]
        [DataType(DataType.Password)]
        public string PasswordConfirmed { get; set; }

    }
}
