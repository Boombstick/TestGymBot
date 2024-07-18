using TestGymBot.Domain;
using TestGymBot.Domain.Abstractions.Services;
using TestGymBot.Domain.Abstractions.Repositories;

namespace TestGymBot.Application.Services
{
    public class PersonPropsService : IPersonPropsService
    {
        private readonly IPersonPropsRepository personPropsRespository;

        public PersonPropsService(IPersonPropsRepository personsRepository)
        {
            personPropsRespository = personsRepository;
        }

        public async Task<List<PersonProps>> GetAllPersonProps()
        {
            var personProps = await personPropsRespository.GetAll();
            return personProps.ToList();
        }

        public async Task<PersonProps> GetPersonProps(Guid id)
        {
            var personProps = await personPropsRespository.Get(id);
            return personProps;
        }
        public async Task<Guid> CreatePersonProps(PersonProps personProps)
        {
            return await personPropsRespository.Create(personProps);
        }

        public async Task<Guid> UpdatePersonProps(PersonProps props)
        {
            return await personPropsRespository.Update(props.Id, props.Weight, props.NeckGirth, props.ChestСircumference, props.ShoulderGirth, props.ArmCircumference
                , props.ForearmGirth, props.WaistCircumference, props.BellyGirth, props.ButtockGirth, props.HipGirth, props.ShinGirth);
        }

        public async Task<Guid> DeletePersonProps(Guid id)
        {
            return await personPropsRespository.Delete(id);
        }
    }
}
