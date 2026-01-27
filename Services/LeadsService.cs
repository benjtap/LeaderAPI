using MongoDB.Driver;
using PaieApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaieApi.Services
{
    public class LeadsService
    {
        private readonly IMongoCollection<Lead> _collection;

        public LeadsService(MongoDbService mongoDbService)
        {
            _collection = mongoDbService.Leads;
        }

        // Get items by specific list type (lead, quotes, followup...)
        public async Task<List<Lead>> GetByListTypeAsync(string userId, string listType)
        {
            var filter = Builders<Lead>.Filter.Eq(x => x.UserId, userId) &
                         Builders<Lead>.Filter.Eq(x => x.ListType, listType);
            
            return await _collection.Find(filter)
                                    .SortByDescending(x => x.CreatedAt)
                                    .ToListAsync();
        }

        // Get ALL leads (maybe for search)
        public async Task<List<Lead>> GetAllAsync(string userId)
        {
            return await _collection.Find(x => x.UserId == userId).ToListAsync();
        }

        public async Task<Lead> CreateAsync(Lead lead)
        {
            // Normalize phone for check
            var normalizedPhone = NormalizePhone(lead.Phone);
            
            // Allow duplicate names, but NOT duplicate phones for same user
            if (!string.IsNullOrEmpty(lead.Phone))
            {
                 var filter = Builders<Lead>.Filter.Eq(x => x.UserId, lead.UserId) &
                              (Builders<Lead>.Filter.Eq(x => x.Phone, lead.Phone) | 
                               Builders<Lead>.Filter.Eq(x => x.Phone, normalizedPhone));

                 var existing = await _collection.Find(filter).FirstOrDefaultAsync();
                 if (existing != null)
                 {
                     return existing; // Return existing instead of duplicate
                 }
            }

            await _collection.InsertOneAsync(lead);
            return lead;
        }

        public async Task UpdateAsync(string id, Lead lead)
        {
            await _collection.ReplaceOneAsync(x => x.Id == id, lead);
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<long> DeleteAllAsync(string userId)
        {
            var result = await _collection.DeleteManyAsync(x => x.UserId == userId);
            return result.DeletedCount;
        }

        public async Task<int> RemoveDuplicatesAsync(string userId)
        {
            var all = await _collection.Find(x => x.UserId == userId).ToListAsync();
            var unique = new HashSet<string>();
            int removed = 0;

            foreach (var item in all)
            {
                // Key calculation: Normalize phone, if empty use name
                string key = NormalizePhone(item.Phone);
                if (string.IsNullOrEmpty(key)) key = "name_" + item.Name;

                if (unique.Contains(key))
                {
                    await DeleteAsync(item.Id);
                    removed++;
                }
                else
                {
                    unique.Add(key);
                }
            }
            return removed;
        }

        // Simple normalization: just digits
        private string NormalizePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return "";
            return new string(phone.Where(char.IsDigit).ToArray());
        }

        // Sync logic: Only add if phone/name doesn't exist
        // Note: For sync, we might want to check across ALL list types or just 'lead'?
        // Usually sync goes to 'leads'.
        public async Task<int> SyncContactsAsync(string userId, List<LeaderApi.Controllers.LeadsController.ContactDto> contacts)
        {
            int added = 0;
            foreach (var c in contacts)
            {
                var name = (c.name != null && c.name.Count > 0) ? c.name[0] : "Unknown";
                var rawPhone = (c.tel != null && c.tel.Count > 0) ? c.tel[0] : "";
                
                if (string.IsNullOrEmpty(rawPhone)) continue;

                var normalizedPhone = NormalizePhone(rawPhone);

                // Check if exists (anywhere for this user)
                // We check both raw and normalized to catch existing records
                var builder = Builders<Lead>.Filter;
                var filter = builder.Eq(x => x.UserId, userId) & 
                             (builder.Eq(x => x.Phone, rawPhone) | builder.Eq(x => x.Phone, normalizedPhone) | builder.Eq(x => x.Name, name));

                var exists = await _collection.Find(filter).AnyAsync();
                
                if (!exists)
                {
                    var newLead = new Lead
                    {
                        UserId = userId,
                        Name = name,
                        Phone = normalizedPhone, // Store normalized!
                        ListType = "lead",
                        Initial = (!string.IsNullOrEmpty(name)) ? name.Substring(0, 1).ToUpper() : "?",
                        Color = "#6200ea",
                        Vip = false,
                        IsIcon = false
                    };
                    await _collection.InsertOneAsync(newLead);
                    added++;
                }
            }
            return added;
        }
        public async Task<Lead> MoveLeadAsync(string userId, string leadId, string targetListType)
        {
            var filter = Builders<Lead>.Filter.Eq(x => x.UserId, userId) & Builders<Lead>.Filter.Eq(x => x.Id, leadId);
            var update = Builders<Lead>.Update.Set(x => x.ListType, targetListType);
            
            // Return the updated document
            var options = new FindOneAndUpdateOptions<Lead> { ReturnDocument = ReturnDocument.After };
            return await _collection.FindOneAndUpdateAsync(filter, update, options);
        }
    }
}
