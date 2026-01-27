using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using PaieApi.Services; 
using System.Threading.Tasks;
using System.Security.Claims;
using PaieApi.Models;

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly LeadsService _leadsService;

        public LeadsController(LeadsService leadsService)
        {
            _leadsService = leadsService;
        }

        private string GetUserId()
        {
             // Fallback for dev/local without full auth
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "default_user";
        }

        [HttpGet]
        public async Task<IActionResult> GetLeads()
        {
            var leads = await _leadsService.GetByListTypeAsync(GetUserId(), "lead");
            return Ok(leads);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLead([FromBody] Lead lead)
        {
            if (lead == null) return BadRequest();

            lead.UserId = GetUserId();
            lead.ListType = "lead"; // Force type
            
            if (string.IsNullOrEmpty(lead.Initial) && !string.IsNullOrEmpty(lead.Name))
            {
                lead.Initial = lead.Name.Substring(0,1).ToUpper();
            }
            if (string.IsNullOrEmpty(lead.Color)) lead.Color = "#6200ea";
            
            await _leadsService.CreateAsync(lead);
            return Ok(lead);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncContacts([FromBody] List<ContactDto> contacts)
        {
            int count = await _leadsService.SyncContactsAsync(GetUserId(), contacts);
            return Ok(new { count = count, message = $"Synced {count} contacts" });
        }

        [HttpPost("cleanup")]
        public async Task<IActionResult> CleanupDuplicates()
        {
            int removed = await _leadsService.RemoveDuplicatesAsync(GetUserId());
            return Ok(new { removed = removed, message = $"Removed {removed} duplicates" });
        }

        [HttpPost("move")]
        public async Task<IActionResult> MoveLead([FromBody] MoveLeadDto dto)
        {
            if (string.IsNullOrEmpty(dto.Id) || string.IsNullOrEmpty(dto.TargetType)) return BadRequest();

            var updated = await _leadsService.MoveLeadAsync(GetUserId(), dto.Id, dto.TargetType);
            if (updated == null) return NotFound("Lead not found");
            
            return Ok(updated);
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllLeads()
        {
            var count = await _leadsService.DeleteAllAsync(GetUserId());
            return Ok(new { message = $"Deleted {count} leads" });
        }

        public class MoveLeadDto
        {
            public string Id { get; set; }
            public string TargetType { get; set; }
        }

        public class ContactDto
        {
            public List<string> name { get; set; }
            public List<string> tel { get; set; }
        }
    }
}
