using TestGymBot.Domain;
using TestGymBot.Domain.Abstractions.Services;
using TestGymBot.Domain.Abstractions.Repositories;

namespace TestGymBot.Application.Services
{
    public class RecordsService : IRecordsService
    {

        //private readonly IRecordsRepository _recordsRepository;

        //public RecordsService(IRecordsRepository recordsRepository)
        //{
        //    _recordsRepository = recordsRepository;
        //}

        //public async Task<List<Record>> GetAllRecords()
        //{
        //    return await _recordsRepository.GetAll();
        //}
        //public async Task<Record> GetRecord(Guid id)
        //{
        //    return await _recordsRepository.Get(id);
        //}

        //public async Task<Guid> CreateRecord(Record record)
        //{
        //    return await _recordsRepository.Create(record);
        //}

        //public async Task<Guid> UpdatePerson(Guid id, long userId, long chatId, string userName, string firstName, string lastName)
        //{
        //    return await _recordsRepository.Update(id, userId, chatId, userName, firstName, lastName);
        //}

        //public async Task<Guid> DeleteRecord(Guid id)
        //{
        //    return await _recordsRepository.Delete(id);
        //}
    }
}
