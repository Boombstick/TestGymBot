using System.Text;
using System.Reflection;
using TestGymBot.Domain;
using TestGymBot.Domain.Attributes;

namespace TestGymBot
{
    public static class ProfileUtils
    {
        public static async Task<string> GetPersonProfile(Person person)
        {

            var record = new UserDataDTO(person.FirstName, person.LastName, person.UserName, person.TotalSession.ToString(),
                $"{person.CurrentSession}/{person.TotalCompletedSession}", person.UserId.ToString());
            StringBuilder builder = new StringBuilder();

            foreach (var item in typeof(UserDataDTO).GetProperties())
                builder.AppendLine($"{item.GetCustomAttribute<RussianNameAttribute>().Name} - {item.GetValue(record)}");


            foreach (var prop in typeof(PersonProps).GetProperties().Where(x=>x.Name!="Id"))
                builder.AppendLine($"{prop.GetCustomAttribute<RussianNameAttribute>().Name} - {prop.GetValue(person.Props)}");
            
            return builder.ToString();
        }
        private record UserDataDTO([property: RussianName("Имя")] string FirstName,
            [property: RussianName("Фамилия")] string LastName,
            [property: RussianName("UserName")] string UserName,
            [property: RussianName("Всего тренировок")] string TotalTraining,
            [property: RussianName("Тренировки")] string Training,
            [property: RussianName("Id")] string UserId);

    }
}
