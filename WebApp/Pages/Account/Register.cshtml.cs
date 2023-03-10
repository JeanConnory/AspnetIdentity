using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using WebApp.Data.Account;
using WebApp.Services;

namespace WebApp.Pages.Account
{
	public class RegisterModel : PageModel
	{
		private readonly UserManager<User> _userManager;
		private readonly IEmailService _emailService;

		public RegisterModel(UserManager<User> userManager, IEmailService emailService)
		{
			_userManager = userManager;
			_emailService = emailService;
		}


		[BindProperty]
		public RegisterViewModel? RegisterViewModel { get; set; }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid) return Page();

			var user = new User
			{
				Email = RegisterViewModel.Email,
				UserName = RegisterViewModel.Email
			};

			var claimDepartment = new Claim("Department", RegisterViewModel.Department);
			var claimPosition = new Claim("Position", RegisterViewModel.Position);

			var result = await _userManager.CreateAsync(user, RegisterViewModel.Password);

			if (result.Succeeded)
			{
				await _userManager.AddClaimAsync(user, claimDepartment);
				await _userManager.AddClaimAsync(user, claimPosition);

				var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

				await _emailService.SendAsync("michaelrhcp@hotmail.com", user.Email, "Please confirm your email",
					$"Please click on this link to confirm your email address: <br /> {confirmationLink}");

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

		[Required]
		public string Department { get; set; }

		[Required]
		public string Position { get; set; }
    }
}
