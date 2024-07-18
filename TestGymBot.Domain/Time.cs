namespace TestGymBot.Domain
{
    public class Time
    {
        public Time()
        {

        }
        public Time(Guid id, string period, DateTime date, List<Person> persons = null)
        {
            Id = id;
            Period = period;
            Date = date;
            Persons = persons;

        }
        public Guid Id { get;  set; }
        public string Period { get;  set; }
        public DateTime Date { get; set; }
        public List<Person> Persons { get; set; } = new();
        public List<Person> PersonQueue { get; set; } = new();


    }
}
