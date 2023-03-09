using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
		private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public string Message { get; set; }

        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
			_userManager = userManager;
		}

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user != null) 
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if(result.Succeeded)
                {
                    this.Message = "Email address is successfully confirm, now you can try to login.";
                    return Page();
                }
            }

            this.Message = "Failed to validate email.";
            return Page();
        }
    }
}