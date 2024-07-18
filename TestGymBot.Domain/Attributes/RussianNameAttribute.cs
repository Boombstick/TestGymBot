namespace TestGymBot.Domain.Attributes
{
    public class RussianNameAttribute : Attribute
    {
        public string Name { get; private set; }
        public RussianNameAttribute(string name)
        {
            Name = name;
        }

    }
}
