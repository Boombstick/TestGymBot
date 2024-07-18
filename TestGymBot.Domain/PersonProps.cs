using TestGymBot.Domain.Attributes;

namespace TestGymBot.Domain
{
    public class PersonProps
    {
        public PersonProps()
        {
                
        }
        public PersonProps(Guid id, int weight, int neckGirth, int chestСircumference, int shoulderGirth, int armCircumference,
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
        public PersonProps(Guid id)
        {
            Id= id;
        }

        public Guid Id { get; set; }
        [RussianName("Вес")]
        public int Weight { get; set; } = 0;                //Вес, кг

        [RussianName("Шея")]
        public int NeckGirth { get; set; } = 0;             //Шея, см

        [RussianName("Грудь")]
        public int ChestСircumference { get; set; } = 0;    //Грудь, см

        [RussianName("Плечи")]
        public int ShoulderGirth { get; set; } = 0;         //Плечи, см

        [RussianName("Руки")]
        public int ArmCircumference { get; set; } = 0;      //Руки, см

        [RussianName("Предплечья")]
        public int ForearmGirth { get; set; } = 0;          //Предплечья, см

        [RussianName("Талия")]
        public int WaistCircumference { get; set; } = 0;    //Талия, см

        [RussianName("Живот")]
        public int BellyGirth { get; set; } = 0;            //Живот, см

        [RussianName("Ягодицы")]
        public int ButtockGirth { get; set; } = 0;          //Ягодицы, см

        [RussianName("Бедро")]
        public int HipGirth { get; set; } = 0;              //Бедро, см

        [RussianName("Голень")]
        public int ShinGirth { get; set; } = 0;             //Голень, см

       
    }

}
