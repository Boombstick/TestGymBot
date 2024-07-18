using System.ComponentModel.DataAnnotations;

namespace TestGymBot.DataAccess.Entities
{
    public class PersonPropsEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        public int Weight { get; set; } = 0;         //Вес, кг
        public int NeckGirth { get; set; } = 0;      //Шея, см
        public int ChestСircumference { get; set; } = 0;   //Грудь, см
        public int ShoulderGirth { get; set; } = 0;       //Плечи, см
        public int ArmCircumference { get; set; } = 0;     //Руки, см
        public int ForearmGirth { get; set; } = 0;       //Предплечья, см
        public int WaistCircumference { get; set; } = 0;    //Талия, см
        public int BellyGirth { get; set; } = 0;          //Живот, см
        public int ButtockGirth { get; set; } = 0;          //Ягодицы, см
        public int HipGirth { get; set; } = 0;           //Бедро, см
        public int ShinGirth { get; set; } = 0;           //Голень, см
        public DateTime DateOfRecording { get; set; } = DateTime.UtcNow;
        public PersonPropsEntity(Guid id, int weight, int neckGirth, int chestСircumference, int shoulderGirth, int armCircumference,
            int forearmGirth, int waistCircumference, int bellyGirth, int buttockGirth, int hipGirth, int shinGirth)
        {
            Id = id;
            Weight = weight;
            NeckGirth = neckGirth;
            ChestСircumference = chestСircumference;
            ShoulderGirth = shoulderGirth;
            ArmCircumference = armCircumference;
            ForearmGirth = forearmGirth;
            WaistCircumference = waistCircumference;
            BellyGirth = bellyGirth;
            ButtockGirth = buttockGirth;
            HipGirth = hipGirth;
            ShinGirth = shinGirth;
        }
        public PersonPropsEntity(Guid id)
        {
            Id = id;
        }
        public PersonPropsEntity()
        {

        }
    }
}
