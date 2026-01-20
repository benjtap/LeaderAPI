using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using LeaderApi.Services;

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetActivities()
        {
            return Ok(InMemoryStore.Activities);
        }

        [HttpPost]
        public IActionResult CreateActivity([FromBody] PaieApi.Models.Activity activity)
        {
            if (activity == null) return BadRequest();
            
            activity.Id = InMemoryStore.Activities.Count + 1;
            activity.Date = DateTime.Now;
            if (string.IsNullOrEmpty(activity.Time)) activity.Time = DateTime.Now.ToString("h:mm tt");
             
            InMemoryStore.Activities.Insert(0, activity);
            return Ok(activity);
        }
    }
}
