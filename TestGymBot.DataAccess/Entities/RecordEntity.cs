using System.ComponentModel.DataAnnotations;

namespace TestGymBot.DataAccess.Entities
{
    public class RecordEntity
    {
        public Guid PersonId { get; set; }
        public Guid TimeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
