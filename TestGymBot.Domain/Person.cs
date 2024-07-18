namespace TestGymBot.Domain

{
    public class Person
    {
        public Person()
        {

        }
        public Person(Guid id, long userId, long chatId, string userName, string firstName = "", string lastName = "")
        {
            Id = id;
            UserId = userId;
            ChatId = chatId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
        }
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int TotalSession { get; set; }
        public int CurrentSession { get; set; }
        public int TotalCompletedSession { get; set; }
        public PersonProps? Props { get; set; }
        public List<Time> Times { get; set; }

        public DaysOfWeek CurrentDay { get; set; }
        public State State { get; set; } = State.Default;

        public override bool Equals(object? obj)
        {
            return obj is Person person &&
                   Id.Equals(person.Id) &&
                   ChatId == person.ChatId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ChatId);
        }
    }
}
