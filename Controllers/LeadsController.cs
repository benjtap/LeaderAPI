using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetLeads()
        {
            var leads = new List<object>
            {
                new { Id = 1, Name = "בנק מזרחי", Phone = "076-804-8860", Initial = "ב", Color = "#3f51b5", Vip = false },
                new { Id = 2, Name = "hr", Phone = "054-508-4222", Initial = "H", Color = "#e0b0ff", Vip = false },
                new { Id = 3, Name = "Bnei", Phone = "052-632-0677", Initial = "B", Color = "#fdd835", Vip = true },
                new { Id = 4, Name = "03-552-9320", Phone = "", Initial = (string)null, IsIcon = true, Color = "#6200ea", Vip = false },
                new { Id = 5, Name = "054-740-7421", Phone = "", Initial = (string)null, IsIcon = true, Color = "#6200ea", Vip = false }
            };

            return Ok(leads);
        }
    }
}
