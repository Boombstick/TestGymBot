
namespace TestGymBot.Domain

{
    public class Record
    {
        public Record(Guid id, string dayName, string time)
        {
            Id = id;
            DayName = dayName;
            Time = time;
        }
        public Guid Id { get; private set; }

        public string DayName { get; private set; }
        public string Time { get; private set; }
        public int PersonCount { get; set; } = 0;
    }
}
