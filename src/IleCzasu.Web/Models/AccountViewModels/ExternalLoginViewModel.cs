using System.ComponentModel.DataAnnotations;

namespace IleCzasu.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required(ErrorMessage = "Pole Email jest wymagane.")]
        [EmailAddress(ErrorMessage = "Niepoprawny adres E-mail.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Pole Nazwa jest wymagane.")]
        [StringLength(15, ErrorMessage = "Nazwa musi zawieraæ minimum {2} i maksimum {1} znaków.", MinimumLength = 3)]
        public string Name { get; set; }
    }
}
