using Microsoft.EntityFrameworkCore;
using TestGymBot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TestGymBot.DataAccess.Configurations
{
    public class PersonPropsConfiguration : IEntityTypeConfiguration<PersonPropsEntity>
    {
        public void Configure(EntityTypeBuilder<PersonPropsEntity> builder)
        {

            builder.HasKey(x => x.Id);
        }
    }
}
