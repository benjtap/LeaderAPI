using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaieApi.Services;
using PaieApi.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Linq;

namespace LeaderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvitationsController : ControllerBase
    {
        private readonly IMongoCollection<Invitation> _invitations;
        private readonly IMongoCollection<Tenant> _tenants;
        private readonly IMongoCollection<Utilisateur> _users;

        public InvitationsController(MongoDbService mongoDbService)
        {
            _invitations = mongoDbService.Invitations;
            _tenants = mongoDbService.Tenants;
            _users = mongoDbService.Utilisateurs;
        }

        [HttpPost]
        public async Task<IActionResult> InviteMembers([FromBody] InviteRequest request)
        {
            var inviterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;

            // Fallback: If tenant_id not in claim (old token), try fetch user
            if (string.IsNullOrEmpty(tenantId))
            {
                var user = await _users.Find(u => u.Id == inviterId).FirstOrDefaultAsync();
                tenantId = user?.TenantId;
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                return BadRequest("You need to belong to a tenant to invite members.");
            }

            if (request.Emails == null || request.Emails.Count == 0)
            {
                 return BadRequest("No emails provided.");
            }

            var results = new List<object>();

            foreach (var email in request.Emails)
            {
                // Check if user already exists
                var existingUser = await _users.Find(u => u.Email == email || u.Username == email).FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    // If user exists and has NO tenant, we can potentially add them directly (or invite to join)
                    if (string.IsNullOrEmpty(existingUser.TenantId))
                    {
                        // TODO: Direct add logic or link logic?
                        // For now, let's assume we treat them as invited
                        // But wait, the user says "invitation link".
                        // Let's create an invitation anyway.
                    }
                    else if (existingUser.TenantId == tenantId)
                    {
                        results.Add(new { email, status = "Already Member" });
                        continue;
                    }
                    else
                    {
                         results.Add(new { email, status = "Already has another Tenant" });
                         continue;
                    }
                }

                // Check pending invitation
                var existingInvite = await _invitations.Find(i => i.Email == email && i.TenantId == tenantId).FirstOrDefaultAsync();
                if (existingInvite != null)
                {
                    results.Add(new { email, status = "Already Invited" });
                    continue;
                }

                var invitation = new Invitation
                {
                    TenantId = tenantId,
                    Email = email,
                    InvitedBy = inviterId,
                    Role = "Member",
                    Status = "Pending",
                    Token = Guid.NewGuid().ToString("N")
                };

                await _invitations.InsertOneAsync(invitation);
                
                // --- MOCK EMAIL SENDING ---
                Console.WriteLine($"[EMAIL MOCK] To: {email}, Link: /register?token={invitation.Token}");
                // --------------------------

                results.Add(new { email, status = "Invited", token = invitation.Token });
            }

            return Ok(new { success = true, results });
        }

        [HttpGet("members")]
        public async Task<IActionResult> GetMembers()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tenantId = User.FindFirst("tenant_id")?.Value;

             if (string.IsNullOrEmpty(tenantId))
            {
                var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();
                tenantId = user?.TenantId;
            }

            if (string.IsNullOrEmpty(tenantId)) return BadRequest("No tenant.");

            // Get actual members (Users)
            // Fetch Tenant to get Member IDs (or query Users by TenantId if we index it)
            // Tenant model has Members list, which is faster if maintained.
            
            var tenant = await _tenants.Find(t => t.Id == tenantId).FirstOrDefaultAsync();
            var memberIds = tenant?.Members ?? new List<string>();

            // Fetch users info
            var users = await _users.Find(u => memberIds.Contains(u.Id)).ToListAsync();
            var membersList = users.Select(u => new 
            {
                id = u.Id,
                name = u.Username, // or Name if we add it
                email = u.Email,
                role = u.Role, // "Admin" or "User" -> Map to "Owner" / "Member"?
                status = "Active",
                isOwner = u.Id == tenant.OwnerId
            }).ToList();

            // Fetch pending invitations
            var invites = await _invitations.Find(i => i.TenantId == tenantId && i.Status == "Pending").ToListAsync();
            var inviteList = invites.Select(i => new
            {
                id = i.Id,
                name = i.Email, // Use email as name for pending
                email = i.Email,
                role = "Member",
                status = "Pending",
                isOwner = false
            }).ToList();

            return Ok(membersList.Concat(inviteList));
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInviteRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest("Token is required");

            var invitation = await _invitations.Find(i => i.Token == request.Token && i.Status == "Pending").FirstOrDefaultAsync();
            if (invitation == null)
            {
                return BadRequest("Invalid or expired invitation.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null) return Unauthorized();

            // Link User to Tenant
            var update = Builders<Utilisateur>.Update
                .Set(u => u.TenantId, invitation.TenantId)
                .Set(u => u.Role, invitation.Role); 

            await _users.UpdateOneAsync(u => u.Id == userId, update);

            // Add to Tenant Members
            var tenantUpdate = Builders<Tenant>.Update.AddToSet(t => t.Members, userId);
            await _tenants.UpdateOneAsync(t => t.Id == invitation.TenantId, tenantUpdate);

            // Mark Invitation Accepted
            var inviteUpdate = Builders<Invitation>.Update.Set(i => i.Status, "Accepted");
            await _invitations.UpdateOneAsync(i => i.Id == invitation.Id, inviteUpdate);

            return Ok(new { success = true, tenantId = invitation.TenantId, message = "Joined tenant successfully" });
        }

        public class InviteRequest
        {
            public List<string> Emails { get; set; }
        }

        public class AcceptInviteRequest
        {
            public string Token { get; set; }
        }
    }
}
