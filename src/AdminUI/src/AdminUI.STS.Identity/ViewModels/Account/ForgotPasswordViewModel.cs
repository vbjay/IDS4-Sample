using AdminUI.STS.Identity.Configuration;
using System.ComponentModel.DataAnnotations;
using AdminUI.Shared.Configuration.Identity;

namespace AdminUI.STS.Identity.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public LoginResolutionPolicy? Policy { get; set; }
        //[Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Username { get; set; }
    }
}






