using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _24LockyLockers.Pages.OperatorPage
{
    [Authorize(Roles = "Operator")]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}