using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IncidentService;

namespace IncidentCrud.Pages.IncidentClient
{
    public class DeleteModel : PageModel
    {
        Service1Client IncidentService = new Service1Client();
        public IActionResult OnGet()
        {
           bool isDeleted=  IncidentService.DeleteIncidentAsync(new Guid(Request.Query["id"])).Result;

           return RedirectToAction("/Index");
        }
    }
}
