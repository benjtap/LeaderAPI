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
    public class ListsController : ControllerBase
    {
        private readonly LeadsService _leadsService;

        public ListsController(LeadsService leadsService)
        {
            _leadsService = leadsService;
        }

        private string GetUserId()
        {
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "default_user";
        }

        [HttpGet("quotes")]
        public async Task<IActionResult> GetQuotes()
        {
             var list = await _leadsService.GetByListTypeAsync(GetUserId(), "quotes");
             return Ok(list);
        }

        [HttpGet("followup")]
        public async Task<IActionResult> GetFollowUp()
        {
             var list = await _leadsService.GetByListTypeAsync(GetUserId(), "followup");
             return Ok(list);
        }

        [HttpGet("notrelevant")]
        public async Task<IActionResult> GetNotRelevant()
        {
             var list = await _leadsService.GetByListTypeAsync(GetUserId(), "notrelevant");
             return Ok(list);
        }

        [HttpGet("closeddeals")]
        public async Task<IActionResult> GetClosedDeals()
        {
             var list = await _leadsService.GetByListTypeAsync(GetUserId(), "closeddeals");
             return Ok(list);
        }

        [HttpPost("{listType}")]
        public async Task<IActionResult> CreateItem(string listType, [FromBody] Lead item)
        {
             if (item == null) return BadRequest();

             item.UserId = GetUserId();
             item.ListType = listType.ToLower();

             if (string.IsNullOrEmpty(item.Initial) && !string.IsNullOrEmpty(item.Name)) 
             {
                 item.Initial = item.Name.Substring(0, 1).ToUpper();
             }
             if (string.IsNullOrEmpty(item.Color)) item.Color = "#6200ea";
            
             // Validate list type if needed, but for now we accept any string as a bucket
             await _leadsService.CreateAsync(item);
             return Ok(item);
        }
    }
}
