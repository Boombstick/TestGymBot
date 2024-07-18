using TestGymBot.Domain;
using TestGymBot.Domain.Abstractions.Services;
using TestGymBot.Domain.Abstractions.Repositories;

namespace TestGymBot.Application.Services
{
    public class TimesService : ITimesService
    {
        private readonly ITimeRepository _timeRepository;
        private readonly ISimpleRecordRepository _recordRepository;

        public TimesService(ITimeRepository recordsRepository, ISimpleRecordRepository recordRepository)
        {
            _timeRepository = recordsRepository;
            _recordRepository = recordRepository;
        }

        public async Task<List<Time>> GetAllTimes(DateTime firstDate, DateTime lastDate)
        {
            var timeEntities = await _timeRepository.GetAll(firstDate, lastDate);
            var times = timeEntities;
            return times.ToList();
        }
        public async Task<Time> GetTime(Guid id)
        {
            var time  = await _timeRepository.Get(id);
            return time;
        }
        public async Task CreateTime(List<string> time, DateTime date)
        {
            await _timeRepository.Create(time, date);
        }
        public async Task AddTimes(List<string> times, DateTime date)
        {
            var timeEntities = times.Select(x => new Time()
            {
                Id = Guid.NewGuid(),
                Period = x,
                Date = new DateTime(date.Year, date.Month, date.Day),
            }).ToList();
            await _timeRepository.AddTimesToDay(timeEntities);
        }
        public async Task DeleteTimes(List<Time> times)
        {
            await _timeRepository.DeleteTimesFromDay(times);
        }
        public async Task<List<Time>> GetAllTimes(DateTime firstDate, DateTime lastDate, DaysOfWeek daysOfWeek)
        {
            var timeEntities = await _timeRepository.GetAllForDay(firstDate, lastDate, daysOfWeek);
            var times = timeEntities;
            return times.ToList();
        }
        public async Task<List<Time>> GetAllTimesWithTrack(DateTime firstDate, DateTime lastDate, DaysOfWeek daysOfWeek)
        {
            var timeEntities = await _timeRepository.GetAllForDayWithTrack(firstDate, lastDate, daysOfWeek);
            var times = timeEntities;
            return times.ToList();
        }
        public async Task UpdateTime(Time time)
        {
            var timeEntity = time;
            await _timeRepository.Update(timeEntity);
        }
        
        public async Task<Guid> DeleteTime(Guid id)
        {
            return await _timeRepository.Delete(id);
        }


        public async Task CreateRecord(Guid personId, Guid timeId)
        {
            await _recordRepository.CreateRecord(personId, timeId);
        }

        public async Task DeleteRecord(Guid personId, Guid timeId)
        {
            await _recordRepository.DeleteRecord(personId, timeId);
        }


    }
}
