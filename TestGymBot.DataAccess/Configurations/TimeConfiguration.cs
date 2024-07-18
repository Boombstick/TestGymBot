using Microsoft.EntityFrameworkCore;
using TestGymBot.DataAccess.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TestGymBot.DataAccess.Configurations
{
    public class TimeConfiguration : IEntityTypeConfiguration<TimeEntity>
    {
        public void Configure(EntityTypeBuilder<TimeEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(u => u.Persons)
                .WithMany(p => p.Times)
                .UsingEntity<RecordEntity>(
                time => time.HasOne<PersonEntity>().WithMany().HasForeignKey(person => person.PersonId),
                person => person.HasOne<TimeEntity>().WithMany().HasForeignKey(time => time.TimeId),
                    j =>
                    {
                        j.Property(x => x.CreatedAt).HasDefaultValue(DateTime.Now);
                        j.ToTable("Records");
                    }
                );
        }
    }
}
