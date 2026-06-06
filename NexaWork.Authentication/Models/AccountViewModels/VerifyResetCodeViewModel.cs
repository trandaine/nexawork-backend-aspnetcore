using System.ComponentModel.DataAnnotations;

namespace NexaWork.Authentication.Models.AccountViewModels
{
    public class VerifyResetCodeViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "The code must be exactly 6 characters.")]
        public string Code { get; set; }
    }
}
