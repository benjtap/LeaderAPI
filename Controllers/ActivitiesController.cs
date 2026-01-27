using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.AspNetCore.Mvc;
using PaieApi.Models;
using PaieApi.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly ActivitiesService _activityService;

        public ActivitiesController(ActivitiesService activityService)
        {
            _activityService = activityService;
        }

        private string GetUserId()
        {
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "default_user";
        }

        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
             var list = await _activityService.GetAllAsync(GetUserId());
             return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] PaieApi.Models.Activity activity)
        {
            if (activity == null) return BadRequest();

            activity.UserId = GetUserId();
            activity.Date = DateTime.Now;
            if (string.IsNullOrEmpty(activity.Time)) activity.Time = DateTime.Now.ToString("h:mm tt");
            activity.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            await _activityService.CreateAsync(activity);
            return Ok();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(string id)
        {
            await _activityService.DeleteAsync(id);
            return Ok();
        }
    }
}
