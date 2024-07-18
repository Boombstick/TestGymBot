using Mapster;
using TestGymBot.Domain;
using Microsoft.EntityFrameworkCore;
using TestGymBot.DataAccess.Entities;
using TestGymBot.Domain.Abstractions.Repositories;


namespace TestGymBot.DataAccess.Repositories
{
    public class TimeRepository : ITimeRepository
    {
        private readonly TgBotDbContext _dbContext;

        public TimeRepository(TgBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<Time?> Get(Guid id)
        {
            var timeEntity = await _dbContext.Times.Include(x => x.Persons).AsTracking().FirstOrDefaultAsync(b => b.Id.Equals(id));
            return timeEntity.Adapt<Time>();
        }
        public async Task<IList<Time>> GetAll(DateTime firstNumber, DateTime lastNumber)
        {
            var timeEntities =  await _dbContext.Times.Where(x => x.Date >= firstNumber.Date && x.Date <= lastNumber.Date).Include(t => t.Persons).ToListAsync();
            return timeEntities.Adapt<IList<Time>>();
        }
        public async Task AddTimesToDay(IEnumerable<Time> times)
        {
            _dbContext.Times.AddRange(times.Adapt<IEnumerable<TimeEntity>>());
            _dbContext.SaveChanges();
        }
        public async Task DeleteTimesFromDay(IEnumerable<Time> times)
        {
            _dbContext.Times.RemoveRange(times.Adapt<TimeEntity>());
            _dbContext.SaveChanges();
        }

        public async Task<IList<Time>> GetAllForDay(DateTime firstNumber, DateTime lastNumber, DaysOfWeek dayOfWeek)
        {
            var timeEntities = await _dbContext.Times.Where(x => x.Date >= firstNumber.Date && x.Date <= lastNumber.Date).Include(x => x.Persons).ToListAsync();
            var times = timeEntities.Where(x => x.Date.DayOfWeek == (DayOfWeek)dayOfWeek);
            return times.Adapt<IList<Time>>();
        }
        public async Task<IList<Time>> GetAllForDayWithTrack(DateTime firstNumber, DateTime lastNumber, DaysOfWeek dayOfWeek)
        {
            var timeEntities = await _dbContext.Times.Where(x => x.Date >= firstNumber.Date && x.Date <= lastNumber.Date).Include(x => x.Persons).ToListAsync();
            var times = timeEntities.Where(x => x.Date.DayOfWeek == (DayOfWeek)dayOfWeek).ToList();
            return times.Adapt<IList<Time>>();
        }

        public async Task Create(IEnumerable<string> time, DateTime date)
        {
            var times = _dbContext.Times.Where(x => x.Date.Day == date.Date.Day && x.Date.Month == date.Date.Month && x.Date.Year == date.Date.Year).ToList();
            _dbContext.Times.RemoveRange(times);
            times = time.Select(x => new TimeEntity()
            {
                Id = Guid.NewGuid(),
                Date = new DateTime(date.Year, date.Month, date.Day),
                Period = x
            }).ToList();

            _dbContext.Times.AddRange(times);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Time time)
        {
            await _dbContext.Times
                 .Where(b => b.Id.Equals(time.Id))
                 .ExecuteUpdateAsync(p => p
                 .SetProperty(b => b.Persons, b => time.Persons.Adapt<List<PersonEntity>>())
                 .SetProperty(b => b.Period, b => time.Period)
                 .SetProperty(b => b.Date, b => time.Date));
        }
        public async Task<Guid> Delete(Guid id)
        {
            await _dbContext.Times.Where(x => x.Id.Equals(id)).ExecuteDeleteAsync();
            return id;
        }


    }
}
