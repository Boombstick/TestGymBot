namespace TestGymBot.Domain.Abstractions.Repositories
{
    public interface IPersonPropsRepository
    {
        Task<Guid> Create(PersonProps person);
        Task<Guid> Delete(Guid id);
        Task<PersonProps?> Get(Guid id);
        Task<IList<PersonProps>> GetAll();
        Task<Guid> Update(Guid id, int weight, int neckGirth, int chestСircumference, int shoulderGirth, int armCircumference, int forearmGirth, int waistCircumference, int bellyGirth, int buttockGirth, int hipGirth, int shinGirth);
    }

}