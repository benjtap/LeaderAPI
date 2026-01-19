using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetActivities()
        {
            var activities = new List<object>
            {
                new {
                    Id = 1,
                    Name = "רוני",
                    Initial = "ר",
                    Color = "#000080", // Navy Blue
                    Type = "incoming",
                    Time = "10:21 AM",
                    Date = DateTime.Now,
                    IsExpanded = false
                },
                new {
                    Id = 2,
                    Name = "רועי",
                    Initial = "ר",
                    Color = "#E6E6FA", // Light Purple
                    TextColor = "#a0a0a0",
                    Type = "incoming",
                    Time = "10:21 AM",
                    Date = DateTime.Now,
                    IsExpanded = false
                },
                new {
                    Id = 3,
                    Name = "אבישי",
                    Initial = "א",
                    Color = "#E6E6FA",
                    TextColor = "#a0a0a0",
                    Type = "missed",
                    Time = "6:51 AM",
                    Date = DateTime.Now,
                    IsExpanded = false
                },
                new {
                    Id = 4,
                    Name = "שלומי",
                    Initial = "ש",
                    Color = "#8A2BE2", // BlueViolet
                    Type = "incoming",
                    Time = "16 Jan",
                    Date = DateTime.Now.AddDays(-2),
                    IsExpanded = false
                },
                new {
                    Id = 5,
                    Name = "*6868",
                    Initial = (string)null, // Icon instead
                    IsIcon = true,
                    Color = "#4B0082", // Indigo
                    Type = "missed",
                    Time = "15 Jan",
                    Date = DateTime.Now.AddDays(-3),
                    IsExpanded = false
                },
                 new {
                    Id = 6,
                    Name = "08-851-2411",
                    Initial = (string)null,
                    IsIcon = true,
                    Color = "#4B0082",
                    Type = "incoming",
                    Time = "15 Jan",
                    Date = DateTime.Now.AddDays(-3),
                    IsExpanded = false
                }
            };

            return Ok(activities);
        }
    }
}
