using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
	public class LoginTwoFactorWithAuthenticatorModel : PageModel
	{
		private readonly SignInManager<User> _signInManager;

		[BindProperty]
		public AuthenticatorMFA AuthenticatorMFA { get; set; }

		public LoginTwoFactorWithAuthenticatorModel(SignInManager<User> signInManager)
		{
			this.AuthenticatorMFA = new AuthenticatorMFA();
			_signInManager = signInManager;
		}

		public void OnGet(bool rememberMe)
		{
			this.AuthenticatorMFA.SecurityCode = string.Empty;
			this.AuthenticatorMFA.RememberMe = rememberMe;
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid) return Page();

			var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(this.AuthenticatorMFA.SecurityCode, this.AuthenticatorMFA.RememberMe, false);


			if (result.Succeeded)
			{
				return RedirectToPage("/Index");
			}
			else
			{
				if (result.IsLockedOut)
				{
					ModelState.AddModelError("Authenticator2FA", "You are locked out.");
				}
				else
				{
					ModelState.AddModelError("Authenticator2FA", "Failed to login.");
				}

				return Page();
			}
		}
	}

	public class AuthenticatorMFA
	{
		[Required]
		[Display(Name = "Code")]
		public string SecurityCode { get; set; }

		public bool RememberMe { get; set; }
	}
}
