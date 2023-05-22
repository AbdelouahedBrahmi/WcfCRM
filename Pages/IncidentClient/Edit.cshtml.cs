using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IncidentService;

namespace IncidentCrud.Pages.IncidentClient
{
    public class EditModel : PageModel
    {
        Service1Client IncidentService = new Service1Client();
        public Incident incident = new Incident();
        public void OnGet()
        {
            try
            {
                Guid recordId = new Guid(Request.Query["id"]);
                incident = IncidentService.GetIncidentAsync(recordId).Result;
            }
            catch (Exception ex)
            {
                Response.Redirect("/Error");
            }
            
        }
    }
}
