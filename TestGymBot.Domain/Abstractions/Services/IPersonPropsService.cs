

namespace TestGymBot.Domain.Abstractions.Services
{
    public interface IPersonPropsService
    {
        Task<Guid> CreatePersonProps(PersonProps personProps);
        Task<Guid> DeletePersonProps(Guid id);
        Task<List<PersonProps>> GetAllPersonProps();
        Task<PersonProps> GetPersonProps(Guid id);
        Task<Guid> UpdatePersonProps(PersonProps props);
    }
}