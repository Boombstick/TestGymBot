using TestGymBot.Domain.Attributes;

namespace TestGymBot.Domain
{
    public enum DaysOfWeek
    {
        [RussianName("По умолчанию")]
        Default,
        [RussianName("Пн")]
        Monday,
        [RussianName("Вт")]
        Tuesday,
        [RussianName("Ср")]
        Wednesday,
        [RussianName("Чт")]
        Thursday,
        [RussianName("Пт")]
        Friday,
        [RussianName("Сб")]
        Saturday
    }
}
