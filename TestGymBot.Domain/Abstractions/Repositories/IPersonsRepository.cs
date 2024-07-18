using Telegram.Bot.Types;

namespace TestGymBot.Domain.Abstractions.Repositories
{
    public interface IPersonsRepository
    {
        Task<Guid> Delete(Guid id);
        Task<Person?> Get(Guid id);
        Task<IList<Person>> GetAll();
        Task<Person?> GetAsNoTracking(Guid id);
        Task<Person?> GetByTelegramUser(User user);
        Task<Person> GetWithProps(Guid id);
        Task<Guid> Update(Guid id, long userId, long chatId, string userName, string firstName, string lastName);
    }
}