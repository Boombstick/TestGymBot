using Microsoft.EntityFrameworkCore;
using TestGymBot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TestGymBot.DataAccess.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<PersonEntity>
    {
        public void Configure(EntityTypeBuilder<PersonEntity> builder)
        {

            builder.HasKey(u => u.Id);

            builder.HasOne(x => x.Props);
        }
    }
}
