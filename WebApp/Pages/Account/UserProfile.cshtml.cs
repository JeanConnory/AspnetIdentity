using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp.Data.Account;

namespace WebApp.Pages.Account
{
	[Authorize]
	public class UserProfileModel : PageModel
	{
		private readonly UserManager<User> _userManager;

		[BindProperty]
		public UserProfileViewModel UserProfile { get; set; }

		[BindProperty]
		public string SuccessMessage { get; set; }

		public User MyProperty { get; set; }

		public UserProfileModel(UserManager<User> userManager)
		{
			_userManager = userManager;
			this.UserProfile = new UserProfileViewModel();
			this.SuccessMessage = string.Empty;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			this.SuccessMessage = string.Empty;

			var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

			this.UserProfile.Email = User.Identity.Name;
			this.UserProfile.Department = departmentClaim?.Value;
			this.UserProfile.Position = positionClaim?.Value;

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid) return Page();

			try
			{
				var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();
				await _userManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, UserProfile.Department));
				await _userManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, UserProfile.Position));
			}
			catch (Exception)
			{
				ModelState.AddModelError("UserProfile", "Error ocurred when saving user profile.");
			}

			this.SuccessMessage = "User profile is saved successfully";

			return Page();
		}

		private async Task<(Data.Account.User, Claim, Claim)> GetUserInfoAsync()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var claims = await _userManager.GetClaimsAsync(user);
			var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
			var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");

			return (user, departmentClaim, positionClaim);
		}
	}

	public class UserProfileViewModel
	{
		public string Email { get; set; }

		[Required]
		public string Department { get; set; }

		[Required]
		public string Position { get; set; }
	}
}
