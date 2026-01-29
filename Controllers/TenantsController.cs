using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaieApi.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires login to create a tenant (the initial user must exist)
    public class TenantsController : ControllerBase
    {
        private readonly TenantService _tenantService;
        private readonly JwtService _jwtService;
        private readonly AuthService _authService;

        public TenantsController(TenantService tenantService, JwtService jwtService, AuthService authService)
        {
            _tenantService = tenantService;
            _jwtService = jwtService;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(dto.CompanyName) || string.IsNullOrEmpty(dto.Industry))
            {
                return BadRequest("Company Name and Industry are required.");
            }

            var tenant = await _tenantService.CreateTenantAsync(userId, dto.CompanyName, dto.Industry, dto.CompanySize);

            // Refetch updated user to generate new token with tenant_id and role
            var updatedUserDto = await _authService.ObtenirUtilisateurDto(User.Identity.Name); // User.Identity.Name is username claim
            // Wait, helper in AuthService might allow getting by ID or we assume username is unique
            
            // To be safe, fetch the RAW user model for token generation
            var userModel = await _authService.ObtenirUtilisateurParId(userId);

            var newToken = _jwtService.GenerateToken(userModel);

            return Ok(new 
            { 
                success = true, 
                tenant = tenant,
                token = newToken, // Client needs to update stored token
                user = updatedUserDto
            });
        }

        public class CreateTenantDto
        {
            public string CompanyName { get; set; }
            public string Industry { get; set; }
            public string CompanySize { get; set; }
        }
    }
}
