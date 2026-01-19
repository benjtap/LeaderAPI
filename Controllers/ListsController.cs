using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListsController : ControllerBase
    {
        [HttpGet("quotes")]
        public IActionResult GetQuotes()
        {
            // Empty for now as per image
            return Ok(new List<object>()); 
        }

        [HttpGet("followup")]
        public IActionResult GetFollowUp()
        {
             var list = new List<object>
            {
                new { Id = 1, Name = "Follow Up 1", Sub = "Call back later", Initial = "F", Color = "#ff9800" },
                new { Id = 2, Name = "Follow Up 2", Sub = "Meeting scheduled", Initial = "M", Color = "#4caf50" }
            };
            return Ok(list);
        }

        [HttpGet("notrelevant")]
        public IActionResult GetNotRelevant()
        {
             return Ok(new List<object>()); 
        }
    }
}
