using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _24LockyLockers.Pages
{
    public class TestRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public TestRolesModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string Message { get; set; } = "";

        public async Task OnGetAsync()
        {
            var client = await _userManager.FindByEmailAsync("client@24lockylockers.com");
            if (client != null)
            {
                var roles = await _userManager.GetRolesAsync(client);
                Message = $"Client roles: {string.Join(", ", roles)}";
            }
            else
            {
                Message = "Client user not found!";
            }
        }
    }
}