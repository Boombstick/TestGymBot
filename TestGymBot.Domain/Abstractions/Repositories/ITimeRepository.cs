namespace TestGymBot.Domain.Abstractions.Repositories
{
    public interface ITimeRepository
    {
        Task AddTimesToDay(IEnumerable<Time> times);
        Task Create(IEnumerable<string> time, DateTime date);
        Task<Guid> Delete(Guid id);
        Task DeleteTimesFromDay(IEnumerable<Time> times);
        Task<Time?> Get(Guid id);
        Task<IList<Time>> GetAll(DateTime firstNumber, DateTime lastNumber);
        Task<IList<Time>> GetAllForDay(DateTime firstNumber, DateTime lastNumber, DaysOfWeek dayOfWeek);
        Task<IList<Time>> GetAllForDayWithTrack(DateTime firstNumber, DateTime lastNumber, DaysOfWeek dayOfWeek);
        Task Update(Time time);
    }
}