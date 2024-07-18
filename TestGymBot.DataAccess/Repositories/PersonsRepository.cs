using Mapster;
using TestGymBot.Domain;
using Telegram.Bot.Types;
using Microsoft.EntityFrameworkCore;
using TestGymBot.Domain.Abstractions.Repositories;
using TestGymBot.DataAccess.Entities;

namespace TestGymBot.DataAccess.Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly TgBotDbContext _dbContext;

        public PersonsRepository(TgBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<Person>> GetAll()
        {
            var personsEntities = await _dbContext.Persons
                .ToListAsync();
            return personsEntities.Adapt<IList<Person>>();
        }

        public async Task<Person?> Get(Guid id)
        {
            var personEntity = await _dbContext.Persons.AsTracking().FirstOrDefaultAsync(b => b.Id.Equals(id));
            return personEntity.Adapt<Person>();
        }

        public async Task<Person?> GetAsNoTracking(Guid id)
        {
            var personEntity = await _dbContext.Persons.FirstOrDefaultAsync(b => b.Id.Equals(id));
            return personEntity.Adapt<Person>();
        }
        public async Task<Person> GetWithProps(Guid id)
        {
            var personEntity = await _dbContext.Persons.Include(x => x.Props).FirstOrDefaultAsync(b => b.Id.Equals(id));
            return personEntity.Adapt<Person>();
        }
        public async Task<Person?> GetByTelegramUser(User user)
        {
            PersonEntity person;
            var personEntity = await _dbContext.Persons.Include(x => x.Props).Include(x => x.Times).FirstOrDefaultAsync(b => b.UserId.Equals(user.Id));
            if (personEntity is null)
            {
                var personFromTelegramUser = new PersonEntity(Guid.NewGuid(), user.Id, user.Id, user.Username, user.FirstName, user.LastName);
                personFromTelegramUser.Props = new PersonPropsEntity(Guid.NewGuid());
                person = personFromTelegramUser;
                Create(person);
            }
            else
                person = personEntity;
            
            return person.Adapt<Person>();
        }

        private async Task<Guid> Create(PersonEntity person)
        {
            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            return person.Id;
        }


        public async Task<Guid> Update(Guid id, long userId, long chatId, string userName, string firstName, string lastName)
        {
            await _dbContext.Persons
                 .Where(b => b.Id.Equals(id))
                 .ExecuteUpdateAsync(p => p
                 .SetProperty(b => b.UserId, b => userId)
                 .SetProperty(b => b.ChatId, b => chatId)
                 .SetProperty(b => b.UserName, b => userName)
                 .SetProperty(b => b.FirstName, b => firstName)
                 .SetProperty(b => b.LastName, b => lastName));
            return id;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _dbContext.Persons
                .Where(b => b.Id.Equals(id))
                .ExecuteDeleteAsync();
            return id;
        }
    }




}

