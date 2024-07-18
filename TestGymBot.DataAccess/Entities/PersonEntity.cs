using System.ComponentModel.DataAnnotations;
using TestGymBot.Domain;

namespace TestGymBot.DataAccess.Entities
{
    public class PersonEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int TotalSession { get; set; } = 0;
        public int CurrentSession { get; set; } = 0;
        public int TotalCompletedSession { get; set; } = 0;
        public Guid PropsId { get; set; }
        public PersonPropsEntity Props { get; set; }
        public List<RecordEntity> Records { get; set; } = new();
        public List<TimeEntity> Times { get; set; } = new();
        public PersonEntity()
        {

        }
        public PersonEntity(Person person)
        {
            Id = person.Id;
            UserName = person.UserName;
            FirstName = person.FirstName;
            LastName = person.LastName;
            FirstName = person.FirstName ?? string.Empty;
            LastName = person.LastName ?? string.Empty;
            PropsId = person.Props.Id;
        }
        public PersonEntity(Guid id, long userId, long chatId, string userName, string firstName = "", string lastName = "")
        {
            Id = id;
            UserId = userId;
            ChatId = chatId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
