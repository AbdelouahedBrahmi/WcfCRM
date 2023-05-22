using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography.Xml;
using IncidentService;

namespace IncidentCrud.Pages.IncidentClient
{
    public class IndexModel : PageModel
    {
        // public Task<Incident[]>? incidents;
        public Incident[] incidents;
        Service1Client serviceIncident = new Service1Client();
        public void OnGet()
        {

           incidents = serviceIncident.GetMultipleRecordAsync().Result;
        }
    }
}
