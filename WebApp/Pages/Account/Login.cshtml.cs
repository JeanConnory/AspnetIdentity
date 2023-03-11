using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
	public class LoginModel : PageModel
	{
		private readonly SignInManager<User> _signInManager;

		public LoginModel(SignInManager<User> signInManager)
		{
			_signInManager = signInManager;
		}

		[BindProperty]
		public CredentialViewModel Credential { get; set; }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid) return Page();

			var result = await _signInManager.PasswordSignInAsync(this.Credential.Email, this.Credential.Password, this.Credential.RememberMe, false);

			if (result.Succeeded)
			{
				return RedirectToPage("/Index");
			}
			else
			{
				if(result.RequiresTwoFactor)
				{
					return RedirectToPage("/Account/LoginTwoFactor", new { Email = this.Credential.Email, RememberMe = this.Credential.RememberMe });
					//return RedirectToPage("/Account/LoginTwoFactorWithAuthenticator", new { RememberMe = this.Credential.RememberMe });
				}
				if (result.IsLockedOut)
				{
					ModelState.AddModelError("Login", "You are locked out.");
				}
				else
				{
					ModelState.AddModelError("Login", "Failed to login.");
				}

				return Page();
			}
		}
	}

	public class CredentialViewModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Remember Me")]
		public bool RememberMe { get; set; }
	}
}
