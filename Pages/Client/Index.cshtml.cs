using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _24LockyLockers.Pages.Client
{
    [Authorize(Roles = "Client")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}