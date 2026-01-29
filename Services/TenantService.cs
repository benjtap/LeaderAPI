using MongoDB.Driver;
using PaieApi.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PaieApi.Services
{
    public class TenantService
    {
        private readonly IMongoCollection<Tenant> _tenants;
        private readonly IMongoCollection<Utilisateur> _utilisateurs;

        public TenantService(MongoDbService mongoDbService)
        {
            _tenants = mongoDbService.Tenants;
            _utilisateurs = mongoDbService.Utilisateurs;
        }

        public async Task<Tenant> CreateTenantAsync(string ownerId, string companyName, string industry, string size)
        {
            var tenant = new Tenant
            {
                Name = companyName,
                Industry = industry,
                CompanySize = size,
                OwnerId = ownerId,
                Members = new List<string> { ownerId },
                CreatedAt = DateTime.UtcNow
            };

            await _tenants.InsertOneAsync(tenant);

            // Update user to link tenant and role (if not already set)
            var update = Builders<Utilisateur>.Update
                .Set(u => u.TenantId, tenant.Id)
                .Set(u => u.Role, "Admin"); // Owner becomes Admin

            await _utilisateurs.UpdateOneAsync(u => u.Id == ownerId, update);

            return tenant;
        }

        public async Task<Tenant> GetTenantAsync(string tenantId)
        {
            return await _tenants.Find(t => t.Id == tenantId).FirstOrDefaultAsync();
        }
    }
}
