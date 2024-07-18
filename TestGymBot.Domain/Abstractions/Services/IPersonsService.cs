
using Telegram.Bot.Types;

namespace TestGymBot.Domain.Abstractions.Services
{
    public interface IPersonsService
    {
        Task CreateRecord(Guid personId, Guid timeId);
        Task<Guid> DeletePerson(Guid id);
        Task DeleteRecord(Guid personId, Guid timeId);
        Task<List<Person>> GetAllPerson();
        Task<Person> GetPerson(Guid id);
        Task<Person> GetPersonByTelegramId(User user);
        Task<Person> GetPersonWithProps(Guid id);
        Task<Guid> UpdatePerson(Guid id, long userId, long chatId, string userName, string firstName, string lastName);
    }
}