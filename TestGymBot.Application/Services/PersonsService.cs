using TestGymBot.Domain;
using Telegram.Bot.Types;
using TestGymBot.Domain.Abstractions.Services;
using TestGymBot.Domain.Abstractions.Repositories;

namespace TestGymBot.Application.Services
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ISimpleRecordRepository _recordRepository;

        public PersonsService(IPersonsRepository personsRepository,ISimpleRecordRepository recordRepository)
        {
            _personsRepository = personsRepository;
            _recordRepository = recordRepository;
        }

        public async Task<List<Person>> GetAllPerson()
        {
            var persons = await _personsRepository.GetAll();

            return persons.ToList();
        }

        public async Task<Person> GetPerson(Guid id)
        {
            var person  = await _personsRepository.Get(id);

            return person;
        }
        public async Task<Person> GetPersonWithProps(Guid id)
        {
            var person = await _personsRepository.GetWithProps(id);
            return person;
        }
        public async Task<Person> GetPersonByTelegramId(User user)
        {
           var person = await _personsRepository.GetByTelegramUser(user);
            return person;
        }

        public async Task<Guid> UpdatePerson(Guid id, long userId, long chatId, string userName, string firstName, string lastName)
        {
            return await _personsRepository.Update(id, userId, chatId, userName, firstName, lastName);
        }

        public async Task<Guid> DeletePerson(Guid id)
        {
            return await _personsRepository.Delete(id);
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
