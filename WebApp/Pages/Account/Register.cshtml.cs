using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
		private readonly UserManager<IdentityUser> _userManager;

		public RegisterModel(UserManager<IdentityUser> userManager)
        {
			_userManager = userManager;
		}


        [BindProperty]
        public RegisterViewModel? RegisterViewModel { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            var user = new IdentityUser
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email
            };

            var result = await _userManager.CreateAsync(user, RegisterViewModel.Password);

            if(result.Succeeded)
            {
                return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return Page();
            }
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

		[Required]
        [DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;
    }
}
