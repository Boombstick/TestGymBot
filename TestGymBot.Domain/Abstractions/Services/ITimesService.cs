
namespace TestGymBot.Domain.Abstractions.Services
{
    public interface ITimesService
    {
        Task AddTimes(List<string> times, DateTime date);
        Task CreateRecord(Guid personId, Guid timeId);
        Task CreateTime(List<string> time, DateTime date);
        Task DeleteRecord(Guid personId, Guid timeId);
        Task<Guid> DeleteTime(Guid id);
        Task DeleteTimes(List<Time> times);
        Task<List<Time>> GetAllTimes(DateTime firstDate, DateTime lastDate);
        Task<List<Time>> GetAllTimes(DateTime firstDate, DateTime lastDate, DaysOfWeek daysOfWeek);
        Task<List<Time>> GetAllTimesWithTrack(DateTime firstDate, DateTime lastDate, DaysOfWeek daysOfWeek);
        Task<Time> GetTime(Guid id);
        Task UpdateTime(Time time);
    }
}