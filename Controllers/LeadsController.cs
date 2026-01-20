using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LeaderApi.Services; 

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLeads()
        {
            return Ok(InMemoryStore.Leads);
        }

        [HttpPost("sync")]
        public IActionResult SyncContacts([FromBody] List<ContactDto> contacts)
        {
            if (contacts == null || contacts.Count == 0) return Ok(new { count = 0 });

            int addedCount = 0;
            foreach (var c in contacts)
            {
                var contactName = (c.name != null && c.name.Count > 0) ? c.name[0] : "Unknown";
                var contactPhone = (c.tel != null && c.tel.Count > 0) ? c.tel[0] : "";

                // Check if exists
                if (!InMemoryStore.Leads.Exists(l => l.Phone == contactPhone || l.Name == contactName))
                {
                    var newLead = new PaieApi.Models.Lead
                    {
                        Id = InMemoryStore.Leads.Count + 1,
                        Name = contactName,
                        Phone = contactPhone,
                        Initial = (!string.IsNullOrEmpty(contactName)) ? contactName.Substring(0, 1).ToUpper() : "?",
                        Color = "#6200ea",
                        Vip = false
                    };
                    InMemoryStore.Leads.Insert(0, newLead);
                    addedCount++;

                    // ALSO CREATE FAKE ACTIVITY for "Recent Call" simulation
                    var newActivity = new PaieApi.Models.Activity
                    {
                        Id = InMemoryStore.Activities.Count + 1,
                        Name = newLead.Name,
                        Initial = newLead.Initial,
                        Color = newLead.Color,
                        Type = "incoming", // Pretend they called us
                        Time = System.DateTime.Now.ToString("h:mm tt"),
                        Date = System.DateTime.Now,
                        IsExpanded = false
                    };
                    InMemoryStore.Activities.Insert(0, newActivity);
                }
            }
            return Ok(new { count = addedCount, message = $"Synced {addedCount} contacts" });
        }

        public class ContactDto
        {
            public List<string> name { get; set; }
            public List<string> tel { get; set; }
        }
    }
}
