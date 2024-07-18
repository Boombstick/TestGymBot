


namespace TestGymBot.Domain.Abstractions.Repositories
{
    public interface IRecordsRepository
    {
        Task CreateRecord(Guid personId, Guid timeId);
        Task DeleteRecord(Guid personId, Guid timeId);
        Task<Record?> GetRecord(Guid personId, Guid timeId);
        Task<IList<Record>> GetAllRecords();
    }
}