using System.ComponentModel.DataAnnotations;

namespace TestGymBot.DataAccess.Entities
{
    public class TimeEntity
    {
        [Key]
        [Required]

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Period { get; set; }

        public List<RecordEntity> Records { get; set; } = new();
        public List<PersonEntity> Persons { get; set; } = new();
    }
}
