using Mapster;
using TestGymBot.Domain;
using Microsoft.EntityFrameworkCore;
using TestGymBot.Domain.Abstractions.Repositories;
using TestGymBot.DataAccess.Entities;

namespace TestGymBot.DataAccess.Repositories
{
    public class PersonPropsRepository : IPersonPropsRepository
    {
        private readonly TgBotDbContext _dbContext;

        public PersonPropsRepository(TgBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<PersonProps>> GetAll()
        {
            var personsPropsEntities = await _dbContext.PersonProps
                .ToListAsync();

            return personsPropsEntities.Adapt<IList<PersonProps>>();
        }

        public async Task<PersonProps?> Get(Guid id)
        {
            var personPropsEntity = await _dbContext.PersonProps.FirstOrDefaultAsync(b => b.Id.Equals(id));

            return personPropsEntity.Adapt<PersonProps>();
        }

        public async Task<Guid> Create(PersonProps personProps)
        {
            await _dbContext.PersonProps.AddAsync(personProps.Adapt<PersonPropsEntity>());
            await _dbContext.SaveChangesAsync();

            return personProps.Id;
        }


        public async Task<Guid> Update(Guid id, int weight, int neckGirth, int chestСircumference, int shoulderGirth, int armCircumference,
            int forearmGirth, int waistCircumference, int bellyGirth, int buttockGirth, int hipGirth, int shinGirth)
        {
            await _dbContext.PersonProps
                 .Where(b => b.Id.Equals(id))
                 .ExecuteUpdateAsync(p => p
                 .SetProperty(b => b.Weight, b => weight)
                 .SetProperty(b => b.NeckGirth, b => neckGirth)
                 .SetProperty(b => b.ChestСircumference, b => chestСircumference)
                 .SetProperty(b => b.ShoulderGirth, b => shoulderGirth)
                 .SetProperty(b => b.ArmCircumference, b => armCircumference)
                 .SetProperty(b => b.ForearmGirth, b => forearmGirth)
                 .SetProperty(b => b.WaistCircumference, b => waistCircumference)
                 .SetProperty(b => b.BellyGirth, b => bellyGirth)
                 .SetProperty(b => b.ButtockGirth, b => buttockGirth)
                 .SetProperty(b => b.HipGirth, b => hipGirth)
                 .SetProperty(b => b.ShinGirth, b => shinGirth));
            return id;
        }

        public async Task<Guid> Delete(Guid id)
        {
            await _dbContext.PersonProps
                .Where(b => b.Id.Equals(id))
                .ExecuteDeleteAsync();
            return id;
        }
    }
}
