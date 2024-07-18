using Mapster;
using TestGymBot.Domain;
using TestGymBot.DataAccess.Entities;


namespace TestGymBot.Application.Configs
{
    public class MapsterConfig
    {
        public MapsterConfig()
        {
            TypeAdapterConfig<PersonEntity, Person>.NewConfig()
               .Map(dest => dest.Times, src => src.Times).MaxDepth(2)
               .Map(dest => dest.Props, src => src.Props).MaxDepth(2);

            TypeAdapterConfig<TimeEntity, Time>.NewConfig()
   .Map(dest => dest.Persons, src => src.Persons).MaxDepth(2);

        }
    }
}
