using MongoDB.Driver;
using PaieApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaieApi.Services
{
    public class ActivitiesService
    {
        private readonly IMongoCollection<Activity> _collection;

        public ActivitiesService(MongoDbService mongoDbService)
        {
            _collection = mongoDbService.Activities;
        }

        public async Task<List<Activity>> GetAllAsync(string userId)
        {
            return await _collection.Find(x => x.UserId == userId)
                                    .SortByDescending(x => x.Timestamp)
                                    .ToListAsync();
        }

        public async Task<Activity> CreateAsync(Activity activity)
        {
            await _collection.InsertOneAsync(activity);
            return activity;
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
