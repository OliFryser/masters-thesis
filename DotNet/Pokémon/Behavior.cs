namespace Pokémon
{
    public class Behavior
    {
        public Behavior(float flowerPercentage, float doorPercentage)
        {
            FlowerPercentage = flowerPercentage;
            DoorPercentage = doorPercentage;
        }

        public float FlowerPercentage { get; }
        public float DoorPercentage { get; }
    }
}