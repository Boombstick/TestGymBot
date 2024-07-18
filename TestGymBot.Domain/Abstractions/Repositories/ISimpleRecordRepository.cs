namespace TestGymBot.Domain.Abstractions.Repositories
{
    public interface ISimpleRecordRepository
    {
        Task CreateRecord(Guid personId, Guid timeId);
        Task DeleteRecord(Guid personId, Guid timeId);
    }
}