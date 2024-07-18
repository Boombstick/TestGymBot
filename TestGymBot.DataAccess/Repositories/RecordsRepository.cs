using Mapster;
using TestGymBot.Domain;
using Microsoft.EntityFrameworkCore;
using TestGymBot.DataAccess.Entities;
using TestGymBot.Domain.Abstractions.Repositories;

namespace TestGymBot.DataAccess.Repositories
{
    public class RecordsRepository : IRecordsRepository, ISimpleRecordRepository
    {
        private readonly TgBotDbContext _dbContext;

        public RecordsRepository(TgBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<IList<Record>> GetAllRecords()
        {
            var recordsEntities = await _dbContext.Records.ToListAsync();

            var records = recordsEntities.Adapt<IList<Record>>();
            return records;
        }
        public async Task<Record?> GetRecord(Guid personId, Guid timeId)
        {
            var recordEntity = await _dbContext.Records.FirstOrDefaultAsync(b => b.PersonId == personId && b.TimeId == timeId);

            var record = recordEntity.Adapt<Record>();
            return record;
        }

        public async Task CreateRecord(Guid personId, Guid timeId)
        {

            var recordEntity = new RecordEntity()
            {
                PersonId = personId,
                TimeId = timeId
            };
            await _dbContext.Records.AddAsync(recordEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRecord(Guid personId, Guid timeId)
        {
            await _dbContext.Records.Where(x => x.PersonId == personId && x.TimeId == timeId).ExecuteDeleteAsync();
        }
        //public async Task<Guid> Update(Guid id, long userId, long chatId, string userName, string firstName, string lastName)
        //{
        //    await _dbContext.Persons
        //         .Where(b => b.Id.Equals(id))
        //         .ExecuteUpdateAsync(p => p
        //         .SetProperty(b => b.UserId, b => userId)
        //         .SetProperty(b => b.ChatId, b => chatId)
        //         .SetProperty(b => b.UserName, b => userName)
        //         .SetProperty(b => b.FirstName, b => firstName)
        //         .SetProperty(b => b.LastName, b => lastName));
        //    return id;
        //}

    }
}
